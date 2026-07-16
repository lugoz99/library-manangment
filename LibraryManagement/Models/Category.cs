using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public class Category : BaseEntity
    {
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public ICollection<SubCategory> SubCategories { get; private set; }
            = new HashSet<SubCategory>();
    }
}