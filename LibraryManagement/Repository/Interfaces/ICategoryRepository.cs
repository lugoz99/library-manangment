using LibraryManagement.Models;

namespace LibraryManagement.Repository.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByIdAsync(Guid id);
        IQueryable<Category> GetAllQueryable();
        Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null);
    
        Task AddAsync(Category category);
        void Update(Category category);
        void Delete(Guid id);
    
        // Persistencia (Unit of Work)
        Task SaveChangesAsync();
    }
}