namespace LibraryManagement.Models.Dtos.Categories
{
    public record CreateSubCategoryDto(string Name, int CategoryId);


    public record SubCategoryResponse(int Id, string Name, int CategoryId);


    public record UpdateSubCategoryDto(string Name, int CategoryId);

}
