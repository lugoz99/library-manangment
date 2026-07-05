// Repository/CategoryRepository.cs

using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Repository
{
    // This class inherits the common CRUD from Repository<Category>
    // We only add the extra method here
    public class CategoryRepository(ApplicationDbContext context)
        : Repository<Category>(context),ICategoryRepository
    {
      // categories by Name
      public async Task<Category?> GetByNameAsync(string name)
      {
          return await DbSet.FirstOrDefaultAsync(c => c.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
      }
    }
}