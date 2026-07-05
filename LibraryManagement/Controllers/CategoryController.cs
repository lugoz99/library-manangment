using FluentResults.Extensions.AspNetCore;
using LibraryManagement.Models.Dtos;
using LibraryManagement.Models.Dtos.Categories;
using LibraryManagement.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(ICategoryService categoryService) : ControllerBase
    {
        // Get all categories from the database
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetAll()
        {
            var result = await categoryService.GetAllCategoriesAsync();

            // Returns 200 OK with data, or maps errors via FluentResultsProfile
            return result.ToActionResult();
        }

        // Get one category by its unique ID
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryResponseDto>> GetById(int id)
        {
            var result = await categoryService.GetCategoryByIdAsync(id);

            // Returns 200 OK, or 404 Not Found using your NotFoundError mapping
            return result.ToActionResult();
        }

        // Create a new category
        [HttpPost]
        public async Task<ActionResult<CategoryResponseDto>> Create([FromBody] CreateCategoryDto dto)
        {
            var result = await categoryService.CreateCategoryAsync(dto);

            // Returns 200 OK with the object, or 409 Conflict if name exists
            return result.ToActionResult();
        }

        // Update an existing category name and description
        [HttpPut("{id:int}")]
        public async Task<ActionResult<CategoryResponseDto>> Update(int id, [FromBody] UpdateCategoryDto dto)
        {
            var result = await categoryService.UpdateCategoryAsync(id, dto);
            return result.ToActionResult();
        }

        // Delete a category from the system
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await categoryService.DeleteCategoryAsync(id);

            // Returns 200 OK on success, or 404 Not Found if id is invalid
            return result.ToActionResult();
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResponseDto<CategoryResponseDto>>> GetPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
            {
                var result = await categoryService.GetPagedCategoriesAsync(pageNumber, pageSize);
                return result.ToActionResult();
            }
    } 
} 
