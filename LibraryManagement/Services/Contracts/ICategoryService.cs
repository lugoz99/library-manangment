using FluentResults;
using LibraryManagement.Models.DTOs;
using LibraryManagement.Models.DTOs.Categories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagement.Common.Pagination;

namespace LibraryManagement.Services.Contracts
{
    public interface ICategoryService
        {
            // Controller-facing REST methods (single, clear signatures)
            Task<Result<IEnumerable<CategoryResponseDto>>> GetAllCategoriesAsync();
            Task<Result<CategoryResponseDto>> GetCategoryByIdAsync(Guid id);
            Task<Result<PagedResult<CategoryResponseDto>>> GetPagedCategoriesAsync(int pageNumber, int pageSize);

            Task<Result<CategoryResponseDto>> CreateCategoryAsync(CreateCategoryDto dto);
            Task<Result<CategoryResponseDto>> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto);
            Task<Result> DeleteCategoryAsync(Guid id);
        }
}