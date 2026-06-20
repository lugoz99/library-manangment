using LibraryManagment.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagment.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            
        }

        // DbSet Properties are defined here for each entity in the application
        public DbSet<Category> Categories { get; set; }

    }
}
