using FluentResults;
using LibraryManagment.Helpers.Errors;
using LibraryManagment.Models;
using LibraryManagment.Models.Dtos;
using LibraryManagment.Models.Dtos.Categories;
using LibraryManagment.Repository.Interfaces;
using LibraryManagment.Repository.IRepository;
using LibraryManagment.Services.Contracts;
using MapsterMapper;

namespace LibraryManagment.Services.Implementation
{
    public class CategoryService(
        ICategoryRepository categoryRepository,
        IMapper mapper)
        : ICategoryService
    {
        // Get all categories and include their related subcategories
        public async Task<Result<IEnumerable<CategoryResponseDto>>> GetAllCategoriesAsync()
        {
            var categories = await categoryRepository.GetAllAsync(c => c.SubCategories);

            // Mapster automatically maps the entities into CategoryResponseDto and populates the HashSet
            var response = mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
            return Result.Ok(response);
        }

        // Get one category by id and include its related subcategories
        public async Task<Result<CategoryResponseDto>> GetCategoryByIdAsync(int id)
        {
            // We pass the id and the lambda expression to load the subcategories for this specific category
            var category = await categoryRepository.GetByIdAsync(id, c => c.SubCategories);

            // If the category is not found, return a FluentResult failure
            if (category == null)
            {
                return Result.Fail<CategoryResponseDto>(
                    new NotFoundError(nameof(Category), id));
            }

            var response = mapper.Map<CategoryResponseDto>(category);
            return Result.Ok(response);
        }

        // Create a new category
        public async Task<Result<CategoryResponseDto>> CreateCategoryAsync(CreateCategoryDto dto)
        {
            // Map the DTO data into a new Category entity
            var category = mapper.Map<Category>(dto);

            await categoryRepository.AddAsync(category);

            // Save changes and handle duplicate name errors using FluentResults
            var saveResult = await categoryRepository.SaveAsync();
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
            // We do not need includes here because we are only updating the category itself
            var category = await categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                return Result.Fail<CategoryResponseDto>(new NotFoundError(nameof(Category), id));
            }

            // Copy the new values from the DTO into the database entity
            mapper.Map(dto, category);

            var saveResult = await categoryRepository.SaveAsync();
            if (saveResult.IsFailed)
            {
                return Result.Fail<CategoryResponseDto>(saveResult.Errors);
            }

            var response = mapper.Map<CategoryResponseDto>(category);
            return Result.Ok(response);
        }

        // Delete a category from the database
        public async Task<Result> DeleteCategoryAsync(int id)
        {
            var category = await categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                return Result.Fail(new NotFoundError(nameof(Category), id));
            }

            categoryRepository.Delete(category);

            var saveResult = await categoryRepository.SaveAsync();
            if (saveResult.IsFailed)
            {
                return saveResult; // Returns the exact database error result
            }

            return Result.Ok();
        }

        // Get a paginated list of categories (without subcategories for better speed)
        public async Task<Result<PagedResponseDto<CategoryResponseDto>>> GetPagedCategoriesAsync(int pageNumber, int pageSize)
        {
            // The repository handles the Skip and Take methods automatically
            var (categories, totalCount) = await categoryRepository.GetPagedAsync(pageNumber, pageSize);

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