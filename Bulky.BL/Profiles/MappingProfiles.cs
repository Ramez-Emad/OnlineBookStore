using AutoMapper;
using Bulky.BL.Models.Companies;
using Bulky.BL.Models.Products;
using Bulky.DataAccess.Entities;

namespace Bulky.BL.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() : base()
        {
            CreateMap<Product, ProductDTO>()
                .ForMember(prodDto => prodDto.Category, opt => opt.MapFrom(prod => prod.Category.Name));

            CreateMap<Product, ProductDetailsDto>()
              .ForMember(prodDto => prodDto.Category, opt => opt.MapFrom(prod => prod.Category.Name));

            CreateMap<ProductDetailsDto, UpsertProductDto>();

            CreateMap<UpsertProductDto, Product>();

            CreateMap<CompanyDto, Company>().ReverseMap();

        }
    }
}
