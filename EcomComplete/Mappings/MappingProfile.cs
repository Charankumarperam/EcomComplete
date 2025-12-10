using AutoMapper;
using DataAccess.Entities;
using Models.DTOs;
namespace EcomComplete.Mappings
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>();
            CreateMap<CreateProduct, Product>();
            CreateMap<UpdateProduct, Product>();
        }
    }
}
