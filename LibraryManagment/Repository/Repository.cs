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
    public class Repository<T>(DbContext context) : IRepository<T> where T : class
    {
        // DbSet represents the specific database table for the class T
        protected readonly DbSet<T> DbSet = context.Set<T>();

        public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        {
            // AsNoTracking() makes the query faster because it is read-only
            IQueryable<T> query = DbSet.AsNoTracking();

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }
            return await query.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int Id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = DbSet.AsNoTracking();

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            // Search dynamically for the property named "Id" (with uppercase I)
            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == Id);
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await DbSet.AnyAsync(predicate);
        }

        public async Task AddAsync(T entity)
        {
            // This only puts the item in memory. It is NOT in the database yet.
            await DbSet.AddAsync(entity);
        }

        public void Delete(T entity)
        {
            // This only marks the item as "deleted" in memory.
            DbSet.Remove(entity);
        }

        public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, object>>? orderBy = null,
            bool descending = false)
        {
            IQueryable<T> query = DbSet.AsNoTracking();

            // Count all rows in the table first
            var totalCount = await query.CountAsync();

            // If the user wants an order, apply it. If not, order by Id.
            if (orderBy != null)
            {
                query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
            }
            else
            {
                query = query.OrderBy(e => EF.Property<int>(e, "Id"));
            }

            // Skip previous items and take only the requested page size
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, totalCount);
        }
    }
}