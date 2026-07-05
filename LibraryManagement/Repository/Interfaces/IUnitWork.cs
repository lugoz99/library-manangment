using FluentResults;
using LibraryManagement.Models;

namespace LibraryManagement.Repository.Interfaces
{
    public interface IUnitWork:IDisposable
    {
        // IDisposable helps the application close the database connection correctly
        IRepository<Category> Categories { get; }
        IRepository<SubCategory> Subcategories { get; }
        // The ONLY method responsible for saving changes to the real database
        Task<Result> SaveAsync();
    }
}
