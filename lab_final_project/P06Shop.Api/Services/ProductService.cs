using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using P06Shop.Api.Data;
using P06Shop.Api.Models;
using P06Shop.Api.Models.Dto;
using P06Shop.Api.Mappers.Interfaces;
using MongoDB.Bson;

namespace P06Shop.Api.Services
{
    public class ProductService
    {
        private readonly MongoDbContext _context;
        private readonly IProductMapper _productMapper;

        public ProductService(MongoDbContext context, IProductMapper productMapper)
        {
            _context = context;
            _productMapper = productMapper;
        }

        public async Task<(bool Success, IEnumerable<GetProductDTO>? Products, string? ErrorMessage)> GetProducts()
        {
            try
            {
                var products = await _context.Products.Find(_ => true).ToListAsync();
                
                if (products == null || !products.Any())
                    return (false, null, "No products found.");

                var productDtos = new List<GetProductDTO>();
                foreach (var product in products)
                {
                    var categories = await GetCategoriesForProduct(product.Id);
                    productDtos.Add(_productMapper.MapToGetProductDTO(product, categories));
                }

                return (true, productDtos, null);
            }
            catch (Exception ex)
            {
                return (false, null, $"Internal server error while retrieving products: {ex.Message}");
            }
        }

        public async Task<(bool Success, GetProductDTO? Product, string? ErrorMessage)> GetProduct(string id)
        {
            try
            {
                var product = await _context.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
                if (product == null)
                    return (false, null, "Product not found.");

                var categories = await GetCategoriesForProduct(product.Id);
                var productDto = _productMapper.MapToGetProductDTO(product, categories);

                return (true, productDto, null);
            }
            catch (Exception ex)
            {
                return (false, null, $"Internal server error while retrieving the product: {ex.Message}");
            }
        }

        public async Task<(bool Success, GetProductDTO? Product, string? ErrorMessage)> CreateProduct(CreateProductDTO productDto)
        {
            try
            {
                var product = _productMapper.MapCreateDTOToProduct(productDto);
                await _context.Products.InsertOneAsync(product);

                foreach (var categoryName in productDto.Categories)
                {
                    var category = await _context.Categories.Find(c => c.Name == categoryName).FirstOrDefaultAsync();
                    
                    if (category == null)
                    {
                        category = new Category
                        {
                            Id = ObjectId.GenerateNewId().ToString(),
                            Name = categoryName,
                            ProductIds = new List<string> { product.Id }
                        };
                        await _context.Categories.InsertOneAsync(category);
                    }
                    else
                    {
                        if (!category.ProductIds.Contains(product.Id))
                        {
                            var update = Builders<Category>.Update.Push(c => c.ProductIds, product.Id);
                            await _context.Categories.UpdateOneAsync(c => c.Id == category.Id, update);
                        }
                    }
                }

                var categories = await GetCategoriesForProduct(product.Id);
                var createdProductDto = _productMapper.MapToGetProductDTO(product, categories);

                return (true, createdProductDto, null);
            }
            catch (Exception ex)
            {
                return (false, null, $"Internal server error while creating the product: {ex.Message}");
            }
        }

        public async Task<(bool Success, GetProductDTO? Product, string? ErrorMessage)> UpdateProduct(string id, UpdateProductDTO updateDto)
        {
            try
            {
                var existingProduct = await _context.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
                if (existingProduct == null)
                    return (false, null, "Product not found.");

                var product = _productMapper.MapUpdateDTOToProduct(id, updateDto, existingProduct);
                var result = await _context.Products.ReplaceOneAsync(p => p.Id == id, product);
                if (result.MatchedCount == 0)
                    return (false, null, "Product not found.");

                if (updateDto.Categories != null)
                {
                    var existingCategories = await _context.Categories.Find(c => c.ProductIds.Contains(id)).ToListAsync();
                    foreach (var category in existingCategories)
                    {
                        if (!updateDto.Categories.Contains(category.Name))
                        {
                            var update = Builders<Category>.Update.Pull(c => c.ProductIds, id);
                            await _context.Categories.UpdateOneAsync(c => c.Id == category.Id, update);
                        }
                    }

                    foreach (var categoryName in updateDto.Categories)
                    {
                        var category = await _context.Categories.Find(c => c.Name == categoryName).FirstOrDefaultAsync();
                        if (category == null)
                        {
                            category = new Category
                            {
                                Id = ObjectId.GenerateNewId().ToString(),
                                Name = categoryName,
                                ProductIds = new List<string> { id }
                            };
                            await _context.Categories.InsertOneAsync(category);
                        }
                        else if (!category.ProductIds.Contains(id))
                        {
                            var update = Builders<Category>.Update.Push(c => c.ProductIds, id);
                            await _context.Categories.UpdateOneAsync(c => c.Id == category.Id, update);
                        }
                    }
                }

                var categories = await GetCategoriesForProduct(product.Id);
                var updatedProductDto = _productMapper.MapToGetProductDTO(product, categories);

                return (true, updatedProductDto, null);
            }
            catch (Exception ex)
            {
                return (false, null, $"Internal server error while updating the product: {ex.Message}");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteProduct(string id)
        {
            try
            {
                var categories = await _context.Categories.Find(c => c.ProductIds.Contains(id)).ToListAsync();
                foreach (var category in categories)
                {
                    var update = Builders<Category>.Update.Pull(c => c.ProductIds, id);
                    await _context.Categories.UpdateOneAsync(c => c.Id == category.Id, update);
                }

                var result = await _context.Products.DeleteOneAsync(p => p.Id == id);
                if (result.DeletedCount == 0)
                    return (false, "Product not found.");

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, $"Internal server error while deleting the product: {ex.Message}");
            }
        }

        private async Task<List<string>> GetCategoriesForProduct(string productId)
        {
            var categories = await _context.Categories
                .Find(c => c.ProductIds.Contains(productId))
                .ToListAsync();
            return categories.Select(c => c.Name).ToList();
        }
    }
} 