using FluentResults;
using LibraryManagement.Helpers.Errors;
using LibraryManagement.Models;
using LibraryManagement.Models.Dtos;
using LibraryManagement.Models.Dtos.Categories;
using LibraryManagement.Repository.Interfaces;
using LibraryManagement.Services.Contracts;
using MapsterMapper;

namespace LibraryManagement.Services.Implementation
{
    public class CategoryService(IUnitWork unitWork, IMapper mapper) : ICategoryService
    {
        // Get all categories and include their related subcategories
        public async Task<Result<IEnumerable<CategoryResponseDto>>> GetAllCategoriesAsync()
        {
            // We access the repository through the unitWork manager to get data with its relations
            var categories = await unitWork.Categories.GetAllAsync(includes: c => c.SubCategories);

            // Mapster automatically converts the collection of entities into DTOs
            var response = mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
            return Result.Ok(response);
        }

        // Get one category by its unique ID and include its related subcategories
        public async Task<Result<CategoryResponseDto>> GetCategoryByIdAsync(int Id)
        {
            // Find the specific category and load its subcategories at the same time
            var category = await unitWork.Categories.GetByIdAsync(Id, c => c.SubCategories);

            // If the category is not found, we stop the execution and return a custom NotFoundError
            if (category == null)
            {
                return Result.Fail<CategoryResponseDto>(new NotFoundError(nameof(Category), Id));
            }

            var response = mapper.Map<CategoryResponseDto>(category);
            return Result.Ok(response);
        }

        // Create a new category in the system
        public async Task<Result<CategoryResponseDto>> CreateCategoryAsync(CreateCategoryDto dto)
        {
            // Check if another category already has the same name (ignoring uppercase/lowercase)
            // This is an excellent use case for ExistsAsync because we only need a Yes/No answer
            bool nameExists = await unitWork.Categories.ExistsAsync(c => c.Name.ToLower() == dto.Name.ToLower());
            if (nameExists)
            {
                return Result.Fail<CategoryResponseDto>($"A category with the name '{dto.Name}' already exists.");
            }

            // Transform the incoming DTO data into a real database entity
            var category = mapper.Map<Category>(dto);

            // Add the new category object to the database context tracked in memory
            await unitWork.Categories.AddAsync(category);

            // Save all changes to the physical database inside a single transaction
            var saveResult = await unitWork.SaveAsync();

            // If the database transaction fails, return the collection of errors
            if (saveResult.IsFailed)
            {
                return Result.Fail<CategoryResponseDto>(saveResult.Errors);
            }

            var response = mapper.Map<CategoryResponseDto>(category);
            return Result.Ok(response);
        }

        // Update an existing category name or description
        public async Task<Result<CategoryResponseDto>> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
        {
            // Look for the category in the database first. 
            // We do not use ExistsAsync here because we actually need the object to modify it.
            var category = await unitWork.Categories.GetByIdAsync(id);

            if (category == null)
            {
                return Result.Fail<CategoryResponseDto>(new NotFoundError(nameof(Category), id));
            }

            // Copy the new modified values from the incoming DTO directly into the database entity
            mapper.Map(dto, category);

            // Persist the changes using the central Unit of Work coordinator
            var saveResult = await unitWork.SaveAsync();
            if (saveResult.IsFailed)
            {
                return Result.Fail<CategoryResponseDto>(saveResult.Errors);
            }

            var response = mapper.Map<CategoryResponseDto>(category);
            return Result.Ok(response);
        }

        // Delete a category completely from the database
        public async Task<Result> DeleteCategoryAsync(int id)
        {
            // Retrieve the category first to verify it exists before trying to remove it
            var category = await unitWork.Categories.GetByIdAsync(id);

            if (category == null)
            {
                return Result.Fail(new NotFoundError(nameof(Category), id));
            }

            // Mark the category status as "Deleted" inside the memory context pool
            unitWork.Categories.Delete(category);

            // Confirm and execute the final deletion SQL script in the database server
            var saveResult = await unitWork.SaveAsync();
            if (saveResult.IsFailed)
            {
                return saveResult; // Return the exact failure object to the controller
            }

            return Result.Ok();
        }

        // Get a paginated list of categories to improve API performance
        public async Task<Result<PagedResponseDto<CategoryResponseDto>>> GetPagedCategoriesAsync(int pageNumber, int pageSize)
        {
            // The repository automatically skips previous rows and takes only the requested page chunk
            var (categories, totalCount) = await unitWork.Categories.GetPagedAsync(pageNumber, pageSize);

            // Build the final response object wrapping the collection metadata and pagination info
            var response = new PagedResponseDto<CategoryResponseDto>
            {
                Items = mapper.Map<IEnumerable<CategoryResponseDto>>(categories),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Result.Ok(response);
        }
    }
}