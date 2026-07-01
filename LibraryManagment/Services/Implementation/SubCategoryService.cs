using FluentResults;
using LibraryManagment.Helpers.Errors;
using LibraryManagment.Models;
using LibraryManagment.Models.Dtos.Categories;
using LibraryManagment.Repository.Interfaces;
using LibraryManagment.Repository.IRepository;
using LibraryManagment.Services.Contracts;
using MapsterMapper;

namespace LibraryManagment.Services.Implementation
{
    public class SubCategoryService(
        ISubCategoryRepository subCategoryRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper) : ISubCategoryService
    {
        // Creates a new subcategory after checking if the parent category exists
        public async Task<Result<SubCategoryResponse>> CreateSubCategoryAsync(CreateSubCategoryDto dto)
        {
            // Check if the parent category exists in the database
            var categoryExists = await categoryRepository.ExistsAsync(c => c.Id == dto.CategoryId);
            if (!categoryExists)
            {
                return Result.Fail<SubCategoryResponse>(new NotFoundError(nameof(Category), dto.CategoryId));
            }

            // Map the DTO data to the SubCategory entity
            var subCategory = mapper.Map<SubCategory>(dto);
            await subCategoryRepository.AddAsync(subCategory);

            // Save changes to the database
            var saveResult = await subCategoryRepository.SaveAsync();
            if (saveResult.IsFailed)
            {
                return Result.Fail<SubCategoryResponse>(saveResult.Errors);
            }

            // Return the created subcategory mapped to a response DTO
            return Result.Ok(mapper.Map<SubCategoryResponse>(subCategory));
        }

        // Deletes a subcategory by its ID
        public async Task<Result> DeleteSubCategoryAsync(int id)
        {
            // Find the subcategory before deleting it
            var subCategory = await subCategoryRepository.GetByIdAsync(id);
            if (subCategory == null)
            {
                return Result.Fail(new NotFoundError(nameof(SubCategory), id));
            }

            // Mark the entity as deleted and save changes
            subCategoryRepository.Delete(subCategory);
            var saveResult = await subCategoryRepository.SaveAsync();
            if (saveResult.IsFailed)
            {
                return saveResult;
            }
            return Result.Ok();
        }

        // Gets all subcategories from the database
        public async Task<Result<IEnumerable<SubCategoryResponse>>> GetAllSubCategoriesAsync()
        {
            var subCategories = await subCategoryRepository.GetAllAsync();
            var response = mapper.Map<IEnumerable<SubCategoryResponse>>(subCategories);
            return Result.Ok(response);
        }

        // Gets a specific subcategory by its ID
        public async Task<Result<SubCategoryResponse>> GetSubCategoryByIdAsync(int id)
        {
            var subCategory = await subCategoryRepository.GetByIdAsync(id);
            if (subCategory == null)
            {
                return Result.Fail(new NotFoundError(nameof(SubCategory), id));
            }
            return Result.Ok(mapper.Map<SubCategoryResponse>(subCategory));
        }

        // Updates an existing subcategory with new details
        public async Task<Result<SubCategoryResponse>> UpdateSubCategoryAsync(int id, UpdateSubCategoryDto dto)
        {
            // Verify that the subcategory exists
            var subCategory = await subCategoryRepository.GetByIdAsync(id);
            if (subCategory == null)
            {
                return Result.Fail(new NotFoundError(nameof(SubCategory), id));
            }

            // Verify that the new parent category exists
            var category = await categoryRepository.GetByIdAsync(dto.CategoryId);
            if (category == null)
            {
                return Result.Fail(new NotFoundError(nameof(Category), dto.CategoryId));
            }

            // Copy the new data from the DTO to the existing entity
            mapper.Map(dto, subCategory);

            // Save the updated data
            var saveResult = await subCategoryRepository.SaveAsync();
            if (saveResult.IsFailed)
            {
                return saveResult;
            }
            return Result.Ok(mapper.Map<SubCategoryResponse>(subCategory));
        }
    }
}