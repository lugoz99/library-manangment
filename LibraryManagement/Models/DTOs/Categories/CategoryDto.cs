using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models.DTOs.Categories
{
    public record CreateCategoryDto(
        [Required(ErrorMessage = "The name is required.")] 
        [StringLength(100, ErrorMessage = "The name cannot exceed 100 characters.")]
        string Name,
        string? Description
    );

    public record UpdateCategoryDto(
        [Required(ErrorMessage = "The name is required.")]
        [StringLength(100, ErrorMessage = "The name cannot exceed 100 characters.")]
        string Name,
        string? Description
    );

    public record CategoryResponseDto(
        Guid Id, // Match with BaseEntity Guid
        string Name,
        string? Description,
        // Using a clean collection for related items. It can be empty, never null.
        IEnumerable<SubCategoryResponseDto> SubCategories
    );
}
