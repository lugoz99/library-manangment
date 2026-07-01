// Repository/CategoryRepository.cs
using LibraryManagment.Data;
using LibraryManagment.Models;
using LibraryManagment.Repository.IRepository;

namespace LibraryManagment.Repository
{
    // This class inherits the common CRUD from Repository<Category>
    // We only add the extra method here
    public class CategoryRepository(ApplicationDbContext context)
        : Repository<Category>(context),ICategoryRepository
    {
        
    }
}