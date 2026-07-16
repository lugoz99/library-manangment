using FluentResults;
using LibraryManagement.Common.Pagination;
using LibraryManagement.Models.DTOs.Categories;

namespace LibraryManagement.Services.Contracts
{
    public interface ISubCategoryService
    {
      

        // Controller-friendly aliases
        Task<Result<IEnumerable<SubCategoryResponseDto>>> GetAllSubCategoriesAsync();
        Task<Result<SubCategoryResponseDto>> GetSubCategoryByIdAsync(Guid id);
        Task<Result<IEnumerable<SubCategoryResponseDto>>> GetSubCategoriesByCategoryIdAsync(Guid categoryId);
        Task<Result<PagedResult<SubCategoryResponseDto>>> GetPagedSubCategoriesAsync(int pageNumber, int pageSize);
        Task<Result<SubCategoryResponseDto>> CreateSubCategoryAsync(CreateSubCategoryDto dto);
        Task<Result<SubCategoryResponseDto>> UpdateSubCategoryAsync(Guid id, UpdateSubCategoryDto dto);
        Task<Result> DeleteSubCategoryAsync(Guid id);
    }
}