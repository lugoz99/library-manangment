using FluentResults;
using LibraryManagement.Common.Pagination;
using LibraryManagement.Helpers.Errors;
using LibraryManagement.Models;
using LibraryManagement.Models.DTOs.Categories;
using LibraryManagement.Repository.Interfaces;
using LibraryManagement.Services.Contracts;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services.Implementation
{
    public class SubCategoryService(
        ISubCategoryRepository subCategoryRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper)
        : ISubCategoryService
    {
        #region Read Methods (Queries)

        public async Task<Result<IEnumerable<SubCategoryResponseDto>>> GetAllSubCategoriesAsync()
        {
            // The repository already includes the parent Category and uses AsNoTracking
            var subCategories = await subCategoryRepository
                .GetAllQueryable()
                .ToListAsync();

            return Result.Ok(subCategories.Adapt<IEnumerable<SubCategoryResponseDto>>());
        }

        public async Task<Result<SubCategoryResponseDto>> GetSubCategoryByIdAsync(Guid id)
        {
            var subCategory = await subCategoryRepository.GetByIdAsync(id);

            if (subCategory is null)
            {
                return Result.Fail(new NotFoundError(nameof(SubCategory), id));
            }

            return Result.Ok(subCategory.Adapt<SubCategoryResponseDto>());
        }

        public async Task<Result<IEnumerable<SubCategoryResponseDto>>> GetSubCategoriesByCategoryIdAsync(Guid categoryId)
        {
            var subCategories = await subCategoryRepository
                .GetAllQueryable()
                .Where(s => s.CategoryId == categoryId)
                .ToListAsync();

            return Result.Ok(subCategories.Adapt<IEnumerable<SubCategoryResponseDto>>());
        }

        public async Task<Result<PagedResult<SubCategoryResponseDto>>> GetPagedSubCategoriesAsync(
            int pageNumber,
            int pageSize)
        {
            var query = subCategoryRepository
                .GetAllQueryable()
                .OrderByDescending(s => s.Name);

            var paged = await query.ToPagedAsync(pageNumber, pageSize);

            var response = new PagedResult<SubCategoryResponseDto>
            {
                Items = paged.Items.Adapt<IEnumerable<SubCategoryResponseDto>>(),
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount
            };

            return Result.Ok(response);
        }

        #endregion

        #region Write Methods (Commands)

        public async Task<Result<SubCategoryResponseDto>> CreateSubCategoryAsync(CreateSubCategoryDto dto)
        {
            // 1. Check Parent Category: Verify if the parent category exists in the database
            // e.g., If CategoryId is 123, check if "History" category exists.
            var parentCategory = await categoryRepository.GetByIdAsync(dto.CategoryId);

            if (parentCategory is null)
            {
                return Result.Fail(new NotFoundError(nameof(Category), dto.CategoryId));
            }

            // 2. Business Rule (No Parent Duplication): A subcategory name cannot be the same as the parent
            // e.g., Category: "History" -> Subcategory cannot be "History".
            if (string.Equals(parentCategory.Name, dto.Name, StringComparison.OrdinalIgnoreCase))
            {
                return Result.Fail(new ConflictError(
                    $"A subcategory cannot have the exact same name as its parent category ('{parentCategory.Name}')."));
            }

            // 3. Check Duplicates: Verify if another sibling subcategory already uses this name
            // e.g., Under "History", we cannot have two subcategories named "World War II".
            if (await subCategoryRepository.ExistsByNameAsync(dto.Name, dto.CategoryId))
            {
                return Result.Fail(new ConflictError($"A subcategory with the name '{dto.Name}' already exists under this category."));
            }

            var subCategory = dto.Adapt<SubCategory>();
            subCategory.Id = Guid.NewGuid();

            await subCategoryRepository.AddAsync(subCategory);
            await subCategoryRepository.SaveChangesAsync();

            // 4. Refresh: Load the subcategory again to include the parent Category details in the response
            var savedSubCategory = await subCategoryRepository.GetByIdAsync(subCategory.Id);

            return Result.Ok(savedSubCategory!.Adapt<SubCategoryResponseDto>());
        }

        public async Task<Result<SubCategoryResponseDto>> UpdateSubCategoryAsync(Guid id, UpdateSubCategoryDto dto)
        {
            var subCategory = await subCategoryRepository.GetByIdAsync(id);

            if (subCategory is null)
            {
                return Result.Fail(new NotFoundError(nameof(SubCategory), id));
            }

            // Get the target category. We start with the current category of this subcategory.
            var targetCategory = subCategory.Category;

            // 1. Check Category Change: If the user moves the subcategory, verify that the new parent category exists
            // e.g., Moving "Database" subcategory from "Programming" (ID: 1) to "Networking" (ID: 2).
            // change o move subcategory to other category
            if (subCategory.CategoryId != dto.CategoryId)
            {
                targetCategory = await categoryRepository.GetByIdAsync(dto.CategoryId);

                if (targetCategory is null)
                {
                    return Result.Fail(new NotFoundError(nameof(Category), dto.CategoryId));
                }
            }

            // 2. Business Rule (No Parent Duplication): The subcategory name cannot be the same as its parent
            // e.g., Category: "Databases" -> Subcategory cannot be "Databases".
            if (string.Equals(targetCategory.Name, dto.Name, StringComparison.OrdinalIgnoreCase))
            {
                return Result.Fail(new ConflictError(
                    $"A subcategory cannot have the exact same name as its parent category ('{targetCategory.Name}')."));
            }

            // 3. Optimization: We only check for duplicates in the database if the name or the parent category changed
            //  ya sea la actual o la nueva a la que se está moviendo.
            // e.g., If the user only changes the Description of "SQL Server", we skip the database duplicate check.
            // if namechanged is true == are equals then is false no cambio
            var nameChanged = !string.Equals(subCategory.Name, dto.Name, StringComparison.OrdinalIgnoreCase);
            // move or not move
            var categoryChanged = subCategory.CategoryId != dto.CategoryId;
            var mustValidateDuplicates = nameChanged || categoryChanged;

            if (mustValidateDuplicates)
            {
                // Verify if the name is already used under the target category (ignore this current subcategory ID)
                // e.g., Check if another subcategory is already named "Oracle" under the target category "Databases".
                if (await subCategoryRepository.ExistsByNameAsync(dto.Name, dto.CategoryId, id))
                {
                    return Result.Fail(new ConflictError($"A subcategory named '{dto.Name}' already exists under the target category."));
                }
            }

            // 4. Map and Save: Copy the changes to the tracked entity and save to the database
            mapper.Map(dto, subCategory);

            // Keep the in-memory navigation property in sync (relevant when CategoryId changed),
            // so we can build the response without a second database roundtrip.
            subCategory.Category = targetCategory;

            subCategoryRepository.Update(subCategory);
            await subCategoryRepository.SaveChangesAsync();

       
            return Result.Ok(subCategory.Adapt<SubCategoryResponseDto>());
        }

        public async Task<Result> DeleteSubCategoryAsync(Guid id)
        {
            // 1. Check Existence: Verify if the subcategory exists before trying to delete it
            var subCategory = await subCategoryRepository.GetByIdAsync(id);

            if (subCategory is null)
            {
                return Result.Fail(new NotFoundError(nameof(SubCategory), id));
            }

            // 2. Optimization: Send only the ID to delete the record without doing extra database queries
            subCategoryRepository.Delete(id);
            await subCategoryRepository.SaveChangesAsync();

            return Result.Ok();
        }

        #endregion
    }
}