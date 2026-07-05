using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Repository.Interfaces;

namespace LibraryManagement.Repository
{
    public class SubCategoryRepository(ApplicationDbContext context) : Repository<SubCategory>(context),
        ISubCategoryRepository
    {

    }
}
