using FluentResults;
using LibraryManagement.Common.Pagination;
using LibraryManagement.Helpers.Errors;
using LibraryManagement.Models;
using LibraryManagement.Models.DTOs.Categories;
using LibraryManagement.Repository.Interfaces;
using LibraryManagement.Services.Contracts;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services.Implementation
{
    public class CategoryService(
        ICategoryRepository repository,
        IMapper mapper)
        : ICategoryService
    {
        #region Métodos de Lectura (Queries)

        public async Task<Result<IEnumerable<CategoryResponseDto>>> GetAllCategoriesAsync()
        {
            var items = await repository
                .GetAllQueryable()
                .AsNoTracking()
                .ToListAsync();

            return Result.Ok(items.Adapt<IEnumerable<CategoryResponseDto>>());
        }

        public async Task<Result<CategoryResponseDto>> GetCategoryByIdAsync(Guid id)
        {
            var category = await repository.GetByIdAsync(id);

            if (category is null)
            {
                return Result.Fail(new NotFoundError(nameof(Category), id));
            }

            return Result.Ok(category.Adapt<CategoryResponseDto>());
        }

        public async Task<Result<PagedResult<CategoryResponseDto>>> GetPagedCategoriesAsync(
            int pageNumber,
            int pageSize)
        {
            var query = repository.GetAllQueryable();

            var paged = await query.ToPagedAsync(pageNumber, pageSize);

            var response = new PagedResult<CategoryResponseDto>()
            {
                Items = paged.Items.Adapt<IEnumerable<CategoryResponseDto>>(),
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount
            };

            return Result.Ok(response);
        }

        #endregion

        #region Métodos de Escritura (Commands)

        public async Task<Result<CategoryResponseDto>> CreateCategoryAsync(CreateCategoryDto dto)
        {
            if (await repository.ExistsByNameAsync(dto.Name))
            {
                return Result.Fail(new ConflictError($"A category with the name '{dto.Name}' already exists."));
            }

            var category = dto.Adapt<Category>();

            await repository.AddAsync(category);
            await repository.SaveChangesAsync();

            return Result.Ok(category.Adapt<CategoryResponseDto>());
        }

        public async Task<Result<CategoryResponseDto>> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto)
        {
            var category = await repository.GetByIdAsync(id);

            if (category is null)
            {
                return Result.Fail(new NotFoundError(nameof(Category), id));
            }

            // OPTIMIZACIÓN: Solo viaja a la base de datos si el nombre realmente cambió en el formulario
            if (!string.Equals(category.Name, dto.Name, StringComparison.OrdinalIgnoreCase))
            {
                if (await repository.ExistsByNameAsync(dto.Name, id))
                {
                    return Result.Fail(new ConflictError($"Another category with the name '{dto.Name}' already exists."));
                }
            }

            mapper.Map(dto, category);
            category.UpdatedAt = DateTime.UtcNow;

            repository.Update(category);
            await repository.SaveChangesAsync();

            return Result.Ok(category.Adapt<CategoryResponseDto>());
        }

        public async Task<Result> DeleteCategoryAsync(Guid id)
        {
            var category = await repository.GetByIdAsync(id);

            if (category is null)
            {
                return Result.Fail(new NotFoundError(nameof(Category), id));
            }

            // OPTIMIZACIÓN: Ahora enviamos solo el 'id' al método Delete del repositorio
            repository.Delete(id);
            await repository.SaveChangesAsync();

            return Result.Ok();
        }

        #endregion
    }
}