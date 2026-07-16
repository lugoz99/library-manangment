using LibraryManagement.Models;

namespace LibraryManagement.Repository.Interfaces
{
    public interface ISubCategoryRepository
    {

        Task<SubCategory?> GetByIdAsync(Guid id);
        
        IQueryable<SubCategory> GetAllQueryable();
        
        Task<bool> ExistsByNameAsync(string name, Guid categoryId, Guid? excludeId = null);

        Task AddAsync(SubCategory subCategory);
        
        void Update(SubCategory subCategory);
        
        void Delete(Guid id);


        Task SaveChangesAsync();

    }
}