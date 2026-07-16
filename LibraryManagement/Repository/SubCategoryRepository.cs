
using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Repository
{
    public class SubCategoryRepository(ApplicationDbContext context) : ISubCategoryRepository
    {
        #region Métodos de Lectura (Queries)

        public async Task<SubCategory?> GetByIdAsync(Guid id)
        {
            return await context.SubCategories
                .Include(s => s.Category)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public IQueryable<SubCategory> GetAllQueryable()
        {
            return context.SubCategories
                .AsNoTracking()
                .Include(s => s.Category);
        }

        // Busca si hay alguna subcategoría con este nombre dentro de la misma categoría, 
        // pero ignora mi propio registro si estamos actualizando
        public async Task<bool> ExistsByNameAsync(string name, Guid categoryId, Guid? excludeId = null)
        {
            var query = context.SubCategories
                .AsNoTracking()
                .Where(s => s.Name == name && s.CategoryId == categoryId);

            if (excludeId.HasValue)
            {
                query = query.Where(s => s.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        #endregion

        #region Métodos de Escritura (Commands)

        public async Task AddAsync(SubCategory subCategory)
        {
            await context.SubCategories.AddAsync(subCategory);
        }

        public void Update(SubCategory subCategory)
        {
            context.SubCategories.Update(subCategory);
        }

        public void Delete(Guid id)
        {
            var subCategory = new SubCategory { Id = id };
            context.SubCategories.Remove(subCategory);
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