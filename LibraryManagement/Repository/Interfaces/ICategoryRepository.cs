using LibraryManagement.Models;

namespace LibraryManagement.Repository.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<Category?> GetByNameAsync(string name);
    }
}