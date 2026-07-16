using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public class SubCategory : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public Guid CategoryId { get; set; }

        public Category Category { get;  set; } = null!;
    }
}