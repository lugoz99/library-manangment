using FluentResults.Extensions.AspNetCore;
using LibraryManagement.Common.Pagination;
using LibraryManagement.Models.DTOs.Categories;
using LibraryManagement.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubCategoryController(ISubCategoryService subCategoryService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubCategoryResponseDto>>> GetAll()
        {
            var result = await subCategoryService.GetAllSubCategoriesAsync();
            return result.ToActionResult();
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<SubCategoryResponseDto>> GetById(Guid id)
        {
            var result = await subCategoryService.GetSubCategoryByIdAsync(id);
            return result.ToActionResult();
        }

        [HttpGet("category/{categoryId:guid}")]
        public async Task<ActionResult<IEnumerable<SubCategoryResponseDto>>> GetByCategoryId(
            Guid categoryId)
        {
            var result = await subCategoryService
                .GetSubCategoriesByCategoryIdAsync(categoryId);

            return result.ToActionResult();
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<SubCategoryResponseDto>>> GetPaged(
            [FromQuery] PaginationParams pagination)
        {

            var result = await subCategoryService.GetPagedSubCategoriesAsync(
                pagination.PageNumber,
                pagination.PageSize);

            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<ActionResult<SubCategoryResponseDto>> Create(
            [FromBody] CreateSubCategoryDto dto)
        {
            var result = await subCategoryService.CreateSubCategoryAsync(dto);
            return result.ToActionResult();
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<SubCategoryResponseDto>> Update(
            Guid id,
            [FromBody] UpdateSubCategoryDto dto)
        {
            var result = await subCategoryService.UpdateSubCategoryAsync(id, dto);
            return result.ToActionResult();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await subCategoryService.DeleteSubCategoryAsync(id);
            return result.ToActionResult();
        }
    }
}