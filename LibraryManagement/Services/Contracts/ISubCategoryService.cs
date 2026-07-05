using FluentResults;
using LibraryManagement.Models.Dtos.Categories;

namespace LibraryManagement.Services.Contracts
{
    public interface ISubCategoryService
    {
        
        Task<Result<IEnumerable<SubCategoryResponse>>> GetAllSubCategoriesAsync();

        Task<Result<SubCategoryResponse>> GetSubCategoryByIdAsync(int id);

        Task<Result<SubCategoryResponse>> CreateSubCategoryAsync(CreateSubCategoryDto dto);

        Task<Result<SubCategoryResponse>> UpdateSubCategoryAsync(int id, UpdateSubCategoryDto dto);

        Task<Result> DeleteSubCategoryAsync(int id);

    }
}
