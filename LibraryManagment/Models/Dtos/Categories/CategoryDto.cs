using System.ComponentModel.DataAnnotations;

namespace LibraryManagment.Models.Dtos.Categories
{
    public record CreateCategoryDto(
        [Required] string Name,
        String? Description
    );

    public record UpdateCategoryDto(
        string Name
    );

    public record CategoryResponseDto(
        int Id,
        string Name,
        string? Description,
        HashSet<SubCategoryResponse?> SubCategories
    );
}
