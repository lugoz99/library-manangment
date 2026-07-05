using FluentResults.Extensions.AspNetCore;
using LibraryManagement.Models.Dtos.Categories;
using LibraryManagement.Services.Implementation;
// Asegúrate de que tus DTOs de SubCategoría estén aquí o ajusta el namespace
using LibraryManagement.Services.Contracts;     // Es mejor buena práctica usar la Interfaz (ISubCategoryService) si la tienes
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // NOTA: Si creaste una interfaz ISubCategoryService, te sugiero cambiar SubCategoryService por ISubCategoryService aquí
    public class SubCategoryController(SubCategoryService subCategoryService) : ControllerBase
    {
        // GET: api/SubCategory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubCategoryResponse>>> GetAll()
        {
            // Asumo que tu servicio tiene un método para subcategorías, por ejemplo: GetAllSubCategoriesAsync
            var result = await subCategoryService.GetAllSubCategoriesAsync();
            return result.ToActionResult();
        }

        // GET: api/SubCategory/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<SubCategoryResponse>> GetById(int id)
        {
            var result = await subCategoryService.GetSubCategoryByIdAsync(id);
            return result.ToActionResult();
        }

        // POST: api/SubCategory
        [HttpPost]
        public async Task<ActionResult<SubCategoryResponse>> Create([FromBody] CreateSubCategoryDto dto)
        {
            var result = await subCategoryService.CreateSubCategoryAsync(dto);
            return result.ToActionResult();
        }

        // PUT: api/SubCategory/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<SubCategoryResponse>> Update(int id, [FromBody] UpdateSubCategoryDto dto)
        {
            var result = await subCategoryService.UpdateSubCategoryAsync(id, dto);
            return result.ToActionResult();
        }

        // DELETE: api/SubCategory/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await subCategoryService.DeleteSubCategoryAsync(id);
            return result.ToActionResult();
        }
    }
}