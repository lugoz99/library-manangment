using FluentResults;
using LibraryManagment.Models.Dtos;
using LibraryManagment.Models.Dtos.Categories;

namespace LibraryManagment.Services.Contracts
{
    public interface ICategoryService
    {
        Task<Result<IEnumerable<CategoryResponseDto>>> GetAllCategoriesAsync();
        Task<Result<CategoryResponseDto>> GetCategoryByIdAsync(int id);
        Task<Result<CategoryResponseDto>> CreateCategoryAsync(CreateCategoryDto dto);
        // Services/Contracts/ICategoryService.cs
        Task<Result<CategoryResponseDto>> UpdateCategoryAsync(int id, UpdateCategoryDto dto);
        Task<Result> DeleteCategoryAsync(int id);
        Task<Result<PagedResponseDto<CategoryResponseDto>>> GetPagedCategoriesAsync(int pageNumber, int pageSize);

    }
}
