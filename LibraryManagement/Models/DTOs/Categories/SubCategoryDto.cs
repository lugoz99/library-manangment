using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace LibraryManagement.Models.DTOs.Categories
{
    public record CreateSubCategoryDto(
        [Required(ErrorMessage = "The subcategory name is required.")]
        [StringLength(100, ErrorMessage = "The name cannot exceed 100 characters.")]
        string Name,

        [Required(ErrorMessage = "The parent Category ID is required.")]
        Guid CategoryId
    );

    public record UpdateSubCategoryDto(
        [Required(ErrorMessage = "The subcategory name is required.")]
        [StringLength(100, ErrorMessage = "The name cannot exceed 100 characters.")]
        string Name,

        [Required(ErrorMessage = "The parent Category ID is required.")]
        Guid CategoryId
    );

    [UsedImplicitly]
    public record SubCategoryResponseDto(
        Guid Id,
        string Name,
        Guid CategoryId,
        string CategoryName
    );
}