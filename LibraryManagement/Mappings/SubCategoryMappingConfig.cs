using LibraryManagement.Models;
using LibraryManagement.Models.DTOs.Categories;
using Mapster;

namespace LibraryManagement.Mappings
{
    public class SubCategoryMappingConfig
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CreateSubCategoryDto, SubCategory>();

            config.NewConfig<SubCategory, SubCategoryResponseDto>()
                .Map(dest => dest.CategoryName,
                    src => src.Category.Name);

            config.NewConfig<UpdateSubCategoryDto, SubCategory>()
                .IgnoreNullValues(true)
                .Ignore(dest => dest.Id);
        }
    }
}