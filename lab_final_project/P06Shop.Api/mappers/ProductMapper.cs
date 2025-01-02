using P06Shop.Api.Mappers.Interfaces;
using P06Shop.Api.Models;
using P06Shop.Api.Models.Dto;
using MongoDB.Driver;
using MongoDB.Bson;

namespace P06Shop.Api.Mappers
{
    public class ProductMapper : IProductMapper
    {
        public GetProductDTO MapToGetProductDTO(Product product, List<string> categories)
        {
            return new GetProductDTO
            {
                Id = product.Id,
                Title = product.Title,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Barcode = product.Barcode,
                ReleaseDate = product.ReleaseDate,
                Categories = categories
            };
        }

        public IEnumerable<GetProductDTO> MapToGetProductDTOs(IEnumerable<Product> products)
        {
            return products.Select(p => MapToGetProductDTO(p, new List<string>()));
        }

        public Product MapCreateDTOToProduct(CreateProductDTO createDTO)
        {
            return new Product
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Title = createDTO.Title,
                Description = createDTO.Description,
                Price = createDTO.Price,
                ImageUrl = createDTO.ImageUrl,
                Barcode = createDTO.Barcode,
                ReleaseDate = createDTO.ReleaseDate
            };
        }

        public Product MapUpdateDTOToProduct(string id, UpdateProductDTO updateDTO, Product existingProduct)
        {
            return new Product
            {
                Id = id,
                Title = updateDTO.Title ?? existingProduct.Title,
                Description = updateDTO.Description ?? existingProduct.Description,
                Price = updateDTO.Price ?? existingProduct.Price,
                ImageUrl = updateDTO.ImageUrl ?? existingProduct.ImageUrl,
                Barcode = updateDTO.Barcode ?? existingProduct.Barcode,
                ReleaseDate = updateDTO.ReleaseDate ?? existingProduct.ReleaseDate
            };
        }
    }
} 