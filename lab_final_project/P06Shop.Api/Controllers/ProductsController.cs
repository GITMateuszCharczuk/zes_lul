using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using P06Shop.Api.Data;
using P06Shop.Api.Models;
using P06Shop.Api.Models.Dto;
using P06Shop.Api.Mappers.Interfaces;
using MongoDB.Bson;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly MongoDbContext _context;
    private readonly IProductMapper _productMapper;

    public ProductsController(MongoDbContext context, IProductMapper productMapper)
    {
        _context = context;
        _productMapper = productMapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetProductDTO>>> GetProducts()
    {
        try
        {
            var products = await _context.Products.Find(_ => true).ToListAsync();
            
            if (products == null || !products.Any())
                return NotFound("No products found.");

            var productDtos = new List<GetProductDTO>();
            foreach (var product in products)
            {
                var categories = await GetCategoriesForProduct(product.Id);
                productDtos.Add(_productMapper.MapToGetProductDTO(product, categories));
            }

            return Ok(productDtos);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error while retrieving products.");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetProductDTO>> GetProduct(string id)
    {
        try
        {
            var product = await _context.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (product == null)
                return NotFound("Product not found.");

            var categories = await GetCategoriesForProduct(product.Id);
            var productDto = _productMapper.MapToGetProductDTO(product, categories);

            return Ok(productDto);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error while retrieving the product.");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GetProductDTO>> CreateProduct(CreateProductDTO productDto)
    {
        try
        {
            var product = _productMapper.MapCreateDTOToProduct(productDto);
            await _context.Products.InsertOneAsync(product);

            // Handle categories
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

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, createdProductDto);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error while creating the product.");
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GetProductDTO>> UpdateProduct(string id, UpdateProductDTO updateDto)
    {
        try
        {
            var existingProduct = await _context.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (existingProduct == null)
                return NotFound("Product not found.");

            var product = _productMapper.MapUpdateDTOToProduct(id, updateDto, existingProduct);
            var result = await _context.Products.ReplaceOneAsync(p => p.Id == id, product);
            if (result.MatchedCount == 0)
                return NotFound("Product not found.");

            // Update categories only if they were provided
            if (updateDto.Categories != null)
            {
                // Remove product from categories that are no longer associated
                var existingCategories = await _context.Categories.Find(c => c.ProductIds.Contains(id)).ToListAsync();
                foreach (var category in existingCategories)
                {
                    if (!updateDto.Categories.Contains(category.Name))
                    {
                        var update = Builders<Category>.Update.Pull(c => c.ProductIds, id);
                        await _context.Categories.UpdateOneAsync(c => c.Id == category.Id, update);
                    }
                }

                // Add product to new categories
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

            return Ok(updatedProductDto);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error while updating the product.");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProduct(string id)
    {
        try
        {
            // Remove product from all categories
            var categories = await _context.Categories.Find(c => c.ProductIds.Contains(id)).ToListAsync();
            foreach (var category in categories)
            {
                var update = Builders<Category>.Update.Pull(c => c.ProductIds, id);
                await _context.Categories.UpdateOneAsync(c => c.Id == category.Id, update);
            }

            var result = await _context.Products.DeleteOneAsync(p => p.Id == id);
            if (result.DeletedCount == 0)
                return NotFound("Product not found.");

            return NoContent();
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error while deleting the product.");
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
