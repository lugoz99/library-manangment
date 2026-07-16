using FluentResults.Extensions.AspNetCore;
using LibraryManagement.Common.Pagination;
using LibraryManagement.Models.DTOs.Categories;
using LibraryManagement.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(ICategoryService categoryService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetAll()
        {
            var result = await categoryService.GetAllCategoriesAsync();
            return result.ToActionResult();
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CategoryResponseDto>> GetById(Guid id)
        {
            var result = await categoryService.GetCategoryByIdAsync(id);
            return result.ToActionResult();
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<CategoryResponseDto>>> GetPaged(
            [FromQuery] PaginationParams pagination)
        {

            var result = await categoryService.GetPagedCategoriesAsync(
                pagination.PageNumber,
                pagination.PageSize);

            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<ActionResult<CategoryResponseDto>> Create(
            [FromBody] CreateCategoryDto dto)
        {
            var result = await categoryService.CreateCategoryAsync(dto);
            return result.ToActionResult();
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<CategoryResponseDto>> Update(
            Guid id,
            [FromBody] UpdateCategoryDto dto)
        {
            var result = await categoryService.UpdateCategoryAsync(id, dto);
            return result.ToActionResult();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await categoryService.DeleteCategoryAsync(id);
            return result.ToActionResult();
        }
    }
}