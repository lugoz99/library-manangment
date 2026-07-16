// Repository/CategoryRepository.cs

using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Repository
{
    // This class inherits the common CRUD from Repository<Category>
    // We only add the extra method here
    public class CategoryRepository(ApplicationDbContext context) : ICategoryRepository
    {
        #region Métodos de Lectura (Queries)

        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await context.Categories
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public IQueryable<Category> GetAllQueryable()
        {
            return context.Categories
                .AsNoTracking()
                .Include(c => c.SubCategories);
        }

        // Busca si hay alguna categoría con este nombre, pero ignora mi propio registro
        public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null)
        {
            var query = context.Categories
                .AsNoTracking()
                .Where(c => c.Name == name);

            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        #endregion

        #region Métodos de Escritura (Commands)

        public async Task AddAsync(Category category)
        {
            await context.Categories.AddAsync(category);
        }

        public void Update(Category category)
        {
            context.Categories.Update(category);
        }

        public void Delete(Guid id)
        {
            var category = new Category { Id = id };
            context.Categories.Remove(category);
        }

        #endregion

        #region Persistencia

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }

        #endregion
    }
}