using LibraryManagment.Data;
using LibraryManagment.Models;
using LibraryManagment.Repository.Interfaces;

namespace LibraryManagment.Repository
{
    public class SubCategoryRepository(ApplicationDbContext context) : Repository<SubCategory>(context),
        ISubCategoryRepository
    {

    }
}
