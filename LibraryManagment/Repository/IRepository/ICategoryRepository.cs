using LibraryManagment.Models;
using FluentResults;

namespace LibraryManagment.Repository.IRepository
{
    public interface ICategoryRepository
    {
        Task<Result<IEnumerable<Category>>> GetCategoriesAsync();

        // Get one category by ID. Returns the category or an error explanation
        Task<Result<Category>> GetCategoryByIdAsync(int id);

        // Check if a category exists by its ID
        Task<bool> CategoryExistsAsync(int id);

        // Check if a category exists by its name
        Task<bool> CategoryExistsAsync(string name);

        // Create a new category. Returns the new object or an error message
        Task<Result<Category>> CreateCategoryAsync(Category category);

        // Update a category using the ID from the URL and the new data
        Task<Result> UpdateCategoryAsync(int id, Category category);
    }
}
