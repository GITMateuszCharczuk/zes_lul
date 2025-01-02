using P06Shop.Api.Models;
using P06Shop.Api.Models.Dto;

namespace P06Shop.Api.Mappers.Interfaces
{
    public interface IProductMapper
    {
        GetProductDTO MapToGetProductDTO(Product product, List<string> categories);
        IEnumerable<GetProductDTO> MapToGetProductDTOs(IEnumerable<Product> products);
        Product MapCreateDTOToProduct(CreateProductDTO createDTO);
        Product MapUpdateDTOToProduct(string id, UpdateProductDTO updateDTO, Product existingProduct);
    }
} 