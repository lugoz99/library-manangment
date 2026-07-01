using FluentResults;
using LibraryManagment.Data;
using LibraryManagment.Helpers.Errors;
using LibraryManagment.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LibraryManagment.Repository
{
    /// <summary>
    /// Generic repository implementation using Entity Framework Core.
    /// </summary>
    public class Repository<T>(ApplicationDbContext context) : IRepository<T> where T : class
    {
        // Protected so child classes can use it if they need custom logic
        protected readonly DbSet<T> DbSet = context.Set<T>();

        public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        {
            // We use AsNoTracking() because this is a read-only operation (better memory usage)
            IQueryable<T> query = DbSet.AsNoTracking();

            // If the user provided includes, apply them to the query dynamically
            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            // Execute the query and return the list with all related data loaded
            return await query.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = DbSet.AsNoTracking();

            // Apply includes if you need to load related data for a specific entity
            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            // We look for the "Id" property dynamically using EF.Property because T is generic
            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            // Simple check to see if any record matches the condition
            return await DbSet.AnyAsync(predicate);
        }

        public async Task AddAsync(T entity)
        {
            // Add the new entity to the context tracking
            await DbSet.AddAsync(entity);
        }

        public void Delete(T entity)
        {
            // Remove the entity from the context tracking
            DbSet.Remove(entity);
        }

        public async Task<Result> SaveAsync()
        {
            try
            {
                // Try to save changes to the database
                await context.SaveChangesAsync();
                return Result.Ok();
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                // Return a failure result if a database constraint (like a duplicate name) is broken
                return Result.Fail(new ConflictError("This name already exists."));
            }
        }

        public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
        {
            // We use AsNoTracking() here too because paging is a read-only view operation
            IQueryable<T> query = DbSet.AsNoTracking();

            // Count all rows first, before applying skip or take
            var totalCount = await query.CountAsync();

            // Calculate the items to skip based on the page number and page size
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        /// <summary>
        /// Helper method to check if the error is a duplicate key/unique constraint violation.
        /// </summary>
        private static bool IsUniqueConstraintViolation(DbUpdateException ex)
        {
            return ex.InnerException?.Message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase) == true;
        }
    }
}