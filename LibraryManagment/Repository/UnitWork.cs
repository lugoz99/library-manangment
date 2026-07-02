using FluentResults;
using LibraryManagment.Data; // Importa tu ApplicationDbContext
using LibraryManagment.Models;
using LibraryManagment.Repository.Interfaces;
using LibraryManagment.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using LibraryManagment.Helpers.Errors;

namespace LibraryManagment.Repository
{
    public class UnitWork : IUnitWork
    {
        private readonly ApplicationDbContext _context;

        public IRepository<Category> Categories { get; }
        public IRepository<SubCategory> Subcategories { get; }

        public UnitWork(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            Categories = new Repository<Category>(_context);
            Subcategories = new Repository<SubCategory>(_context);
        }

        public async Task<Result> SaveAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                return Result.Ok();
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                return Result.Fail(new ConflictError("This record already exists."));
            }
        }

        // Helper para errores de duplicación (SQL Server, PostgreSQL, SQLite)
        private static bool IsUniqueConstraintViolation(DbUpdateException ex)
        {
            return ex.InnerException?.Message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase) == true
                || ex.InnerException?.Message.Contains("unique constraint", StringComparison.OrdinalIgnoreCase) == true;
        }

        // 5. Cierras la conexión a la base de datos de forma segura
        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}