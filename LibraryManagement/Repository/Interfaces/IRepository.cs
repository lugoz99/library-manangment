using System.Linq.Expressions;

namespace LibraryManagement.Repository.Interfaces
{
    // T is a generic parameter. It represents any entity class (like Book or User).
    public interface IRepository<T> where T : class
    {
        // Get all items from the database and include related tables
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);

        // Find one single item by its Identifier number
        Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);

        // Check if an item exists using a condition
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

        // Add a new item to the memory context (not saved in database yet)
        Task AddAsync(T entity);

        // Mark an item to be deleted from the database
        void Delete(T entity);

        // Get a small list of items for pages (Example: page 1 with 10 items)
        Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, object>>? orderBy = null,
            bool descending = false);
    }
}