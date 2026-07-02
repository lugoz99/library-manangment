using FluentResults;
using LibraryManagment.Models.Dtos;
using LibraryManagment.Models.Dtos.Categories;

namespace LibraryManagment.Services.Contracts
{
    public interface ISubCategoryService
    {
        
        Task<Result<IEnumerable<SubCategoryResponse>>> GetAllSubCategoriesAsync();

        Task<Result<SubCategoryResponse>> GetSubCategoryByIdAsync(int Id);

        Task<Result<SubCategoryResponse>> CreateSubCategoryAsync(CreateSubCategoryDto dto);

        Task<Result<SubCategoryResponse>> UpdateSubCategoryAsync(int Id, UpdateSubCategoryDto dto);

        Task<Result> DeleteSubCategoryAsync(int Id);

    }
}
