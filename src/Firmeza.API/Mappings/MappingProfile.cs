using AutoMapper;
using Firmeza.Admin.Models;
using Firmeza.API.DTOs;

namespace Firmeza.API.Mappings;

public class MappingProfile : Profile
{
  public MappingProfile()
  {
    // Products
    CreateMap<Product, ProductDto>();

    CreateMap<CreateProductDto, Product>()
      .ForMember(d => d.Id, o => o.Ignore())
      .ForMember(d => d.IsActive, o => o.Ignore())
      .ForMember(d => d.CreatedAt, o => o.Ignore())
      .ForMember(d => d.DeletedAt, o => o.Ignore());

    CreateMap<UpdateProductDto, Product>()
      .ForMember(d => d.Id, o => o.Ignore())
      .ForMember(d => d.CreatedAt, o => o.Ignore())
      .ForMember(d => d.DeletedAt, o => o.Ignore());

    // Clients
    CreateMap<Client, ClientDto>();

    CreateMap<CreateClientDto, Client>()
      .ForMember(d => d.Id, o => o.Ignore())
      .ForMember(d => d.CreatedAt, o => o.Ignore())
      .ForMember(d => d.DeletedAt, o => o.Ignore());

    CreateMap<UpdateClientDto, Client>()
      .ForMember(d => d.Id, o => o.Ignore())
      .ForMember(d => d.CreatedAt, o => o.Ignore())
      .ForMember(d => d.DeletedAt, o => o.Ignore());

    // Sales
    CreateMap<Sale, SaleDto>()
      .ForMember(d => d.ClientName, o => o.MapFrom(s => s.Client.FullName))
      .ForMember(d => d.Items, o => o.MapFrom(s => s.SaleDetails));

    CreateMap<SaleDetail, SaleDetailDto>()
      .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name))
      .ForMember(d => d.Subtotal, o => o.MapFrom(s => s.Quantity * s.UnitPrice));

    CreateMap<SaleItemDto, SaleItemInput>();
  }
}
