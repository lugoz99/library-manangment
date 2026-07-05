using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // =====================================================
        // DBSETS
        // =====================================================

        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }

        // =====================================================
        // MODEL CONFIGURATION
        // =====================================================

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Category

            modelBuilder.Entity<Category>()
                .Property<DateTime>("CreatedAt");

            modelBuilder.Entity<Category>()
                .Property<DateTime>("UpdatedAt");

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique();
        }

        // =====================================================
        // AUDIT FIELDS (SHADOW PROPERTIES)
        // =====================================================

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries()
                .Where(e =>
                    e.State == EntityState.Added ||
                    e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                var entityType = entry.Context.Model
                    .FindEntityType(entry.Entity.GetType());

                var createdProperty =
                    entityType?.FindProperty("CreatedAt");

                var updatedProperty =
                    entityType?.FindProperty("UpdatedAt");

                if (entry.State == EntityState.Added)
                {
                    if (createdProperty != null)
                    {
                        entry.Property("CreatedAt")
                            .CurrentValue = DateTime.UtcNow;
                    }

                    if (updatedProperty != null)
                    {
                        entry.Property("UpdatedAt")
                            .CurrentValue = DateTime.UtcNow;
                    }
                }

                if (entry.State == EntityState.Modified)
                {
                    if (updatedProperty != null)
                    {
                        entry.Property("UpdatedAt")
                            .CurrentValue = DateTime.UtcNow;
                    }
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}