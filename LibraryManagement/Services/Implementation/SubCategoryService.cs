using FluentResults;
using LibraryManagement.Helpers.Errors;
using LibraryManagement.Models;
using LibraryManagement.Models.Dtos.Categories;
using LibraryManagement.Repository.Interfaces;
using LibraryManagement.Services.Contracts;
using MapsterMapper;

namespace LibraryManagement.Services.Implementation
{
    public class SubCategoryService(
        IUnitWork unitWork, // 💡 NEW: We inject the central Unit of Work instead of multiple repositories
        IMapper mapper) : ISubCategoryService
    {
        // Creates a new subcategory after checking if the parent category exists
        public async Task<Result<SubCategoryResponse>> CreateSubCategoryAsync(CreateSubCategoryDto dto)
        {
            // Check if the parent category exists in the database using the unitWork manager
            var categoryExists = await unitWork.Categories.ExistsAsync(c => c.Id == dto.CategoryId);
            if (!categoryExists)
            {
                return Result.Fail<SubCategoryResponse>(new NotFoundError(nameof(Category), dto.CategoryId));
            }

            // Map the incoming DTO data into a new SubCategory database entity
            var subCategory = mapper.Map<SubCategory>(dto);

            // Add the new subcategory to the memory context
            await unitWork.Subcategories.AddAsync(subCategory);

            // 💡 IMPORTANT: We use the Unit of Work to save changes to the real database
            var saveResult = await unitWork.SaveAsync();
            if (saveResult.IsFailed)
            {
                return Result.Fail<SubCategoryResponse>(saveResult.Errors);
            }

            // Return the newly created subcategory mapped to a response DTO
            return Result.Ok(mapper.Map<SubCategoryResponse>(subCategory));
        }

        // Deletes an existing subcategory by its ID number
        public async Task<Result> DeleteSubCategoryAsync(int id)
        {
            // Look for the subcategory in the database before trying to delete it
            var subCategory = await unitWork.Subcategories.GetByIdAsync(id);
            if (subCategory == null)
            {
                return Result.Fail(new NotFoundError(nameof(SubCategory), id));
            }

            // Mark the entity as "Deleted" inside the database context in memory
            unitWork.Subcategories.Delete(subCategory);

            // Confirm and execute the deletion in the real database using the Unit of Work
            var saveResult = await unitWork.SaveAsync();
            if (saveResult.IsFailed)
            {
                return saveResult; // Return the exact database error result
            }
            return Result.Ok();
        }

        // Retrieves all subcategories from the database
        public async Task<Result<IEnumerable<SubCategoryResponse>>> GetAllSubCategoriesAsync()
        {
            // Get the full list using the subcategories repository from unitWork
            var subCategories = await unitWork.Subcategories.GetAllAsync();

            var response = mapper.Map<IEnumerable<SubCategoryResponse>>(subCategories);
            return Result.Ok(response);
        }

        // Retrieves a specific subcategory by its unique ID
        public async Task<Result<SubCategoryResponse>> GetSubCategoryByIdAsync(int Id)
        {
            // Search for the single record using the unitWork manager
            var subCategory = await unitWork.Subcategories.GetByIdAsync(Id);
            if (subCategory == null)
            {
                return Result.Fail(new NotFoundError(nameof(SubCategory), Id));
            }
            return Result.Ok(mapper.Map<SubCategoryResponse>(subCategory));
        }

        // Updates an existing subcategory details and its parent category link
        public async Task<Result<SubCategoryResponse>> UpdateSubCategoryAsync(int Id, UpdateSubCategoryDto dto)
        {
            // Verify that the subcategory exists in our system first
            var subCategory = await unitWork.Subcategories.GetByIdAsync(Id);
            if (subCategory == null)
            {
                return Result.Fail(new NotFoundError(nameof(SubCategory), Id));
            }

            // Verify that the new parent category exists before making the link
            var category = await unitWork.Categories.GetByIdAsync(dto.CategoryId);
            if (category == null)
            {
                return Result.Fail(new NotFoundError(nameof(Category), dto.CategoryId));
            }

            // Copy the modified data from the DTO into the existing database entity
            mapper.Map(dto, subCategory);

            // Save the updated data using the central Unit of Work save method
            var saveResult = await unitWork.SaveAsync();
            if (saveResult.IsFailed)
            {
                return saveResult;
            }
            return Result.Ok(mapper.Map<SubCategoryResponse>(subCategory));
        }
    }
}