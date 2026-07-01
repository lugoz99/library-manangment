using FluentResults;
using System.Linq.Expressions;

namespace LibraryManagment.Repository.Interfaces
{
    /// <summary>
    /// Generic repository interface for basic CRUD and advanced queries.
    /// </summary>
    public interface IRepository<T> where T : class
    {
        // Get all entities and optionally include related tables (1 to * relationships)
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);

        // Get a single entity by ID and optionally include related tables
        Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);

        // Check if an entity exists based on a condition
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

        // Add a new entity to the database context
        Task AddAsync(T entity);

        // Mark an entity for deletion
        void Delete(T entity);

        // Save all changes to the database and return a FluentResult
        Task<Result> SaveAsync();

        // Get a paginated list of entities for better performance
        Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);
    }
}