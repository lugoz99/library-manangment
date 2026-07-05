using FluentResults;
using LibraryManagement.Models.Dtos;
using LibraryManagement.Models.Dtos.Categories;

namespace LibraryManagement.Services.Contracts
{
    public interface ICategoryService
    {
        Task<Result<IEnumerable<CategoryResponseDto>>> GetAllCategoriesAsync();
        Task<Result<CategoryResponseDto>> GetCategoryByIdAsync(int Id);
        Task<Result<CategoryResponseDto>> CreateCategoryAsync(CreateCategoryDto dto);
        // Services/Contracts/ICategoryService.cs
        Task<Result<CategoryResponseDto>> UpdateCategoryAsync(int Id, UpdateCategoryDto dto);
        Task<Result> DeleteCategoryAsync(int Id);
        Task<Result<PagedResponseDto<CategoryResponseDto>>> GetPagedCategoriesAsync(int pageNumber, int pageSize);

    }
}
