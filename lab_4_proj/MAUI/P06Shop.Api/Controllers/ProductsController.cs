using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using P06Shop.Api.Data;
using P06Shop.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using P06Shop.Api.Models.Dto;
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly MongoDbContext _context;

    public ProductsController(MongoDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<ServiceResponse<IEnumerable<Product>>>> GetProducts()
    {
        var response = new ServiceResponse<IEnumerable<Product>>();
        Console.WriteLine("Received request to get all products.");
        try
        {
            var products = await _context.Products.Find(_ => true).ToListAsync();
            
            if (products == null || !products.Any())
            {
                response.Success = false;
                response.Message = "No products found.";
                Console.WriteLine("No products found.");
                return NotFound(response);
            }

            response.Data = products;
            response.Success = true;
            response.Message = "Products retrieved successfully.";
            Console.WriteLine($"Retrieved {products.Count} products successfully.");
            return Ok(response);
        }
        catch (Exception ex)
        {
            // Log the exception (ex) here if needed
            response.Success = false;
            response.Message = "Internal server error while retrieving products.";
            Console.WriteLine($"Error retrieving products: {ex.Message}");
            return StatusCode(500, response);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResponse<Product>>> GetProduct(int id)
    {
        var response = new ServiceResponse<Product>();
        Console.WriteLine($"Received request to get product with ID: {id}");
        try
        {
            var product = await _context.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (product == null)
            {
                response.Success = false;
                response.Message = "Product not found.";
                Console.WriteLine($"Product with ID: {id} not found.");
                return NotFound(response);
            }
            response.Data = product;
            response.Success = true;
            response.Message = "Product retrieved successfully.";
            Console.WriteLine($"Product with ID: {id} retrieved successfully.");
            return Ok(response);
        }
        catch (Exception ex)
        {
            // Log the exception (ex) here if needed
            response.Success = false;
            response.Message = "Internal server error while retrieving the product.";
            Console.WriteLine($"Error retrieving product with ID: {id}. Exception: {ex.Message}");
            return StatusCode(500, response);
        }
    }

    [HttpPost]
    public async Task<ActionResult<ServiceResponse<Product>>> CreateProduct(CreateProductDTO productDto)
    {
        var response = new ServiceResponse<Product>();
        Console.WriteLine("Received request to create a new product.");
        try
        {
            var product = new Product
            {
                Id = new Random().Next(1, int.MaxValue), // Generates a random integer that can be used as an ID
                Title = productDto.Title,
                Description = productDto.Description,
                Barcode = productDto.Barcode,
                Price = productDto.Price,
                ReleaseDate = productDto.ReleaseDate
            };

            await _context.Products.InsertOneAsync(product);
            response.Data = product;
            response.Success = true;
            response.Message = "Product created successfully.";
            Console.WriteLine($"Product created successfully with ID: {product.Id}");
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, response);
        }
        catch (Exception ex)
        {
            // Log the exception (ex) here if needed
            response.Success = false;
            response.Message = "Internal server error while creating the product.";
            Console.WriteLine($"Error creating product: {ex.Message}");
            return StatusCode(500, response);
        }
    }

    [HttpPut]
    public async Task<ActionResult<ServiceResponse<Product>>> UpdateProduct(Product updatedProduct)
    {
        var response = new ServiceResponse<Product>();
        Console.WriteLine($"Received request to update product with ID: {updatedProduct.Id}");
        try
        {
            var result = await _context.Products.ReplaceOneAsync(p => p.Id == updatedProduct.Id, updatedProduct);
            if (result.MatchedCount == 0)
            {
                response.Success = false;
                response.Message = "Product not found.";
                Console.WriteLine($"Product with ID: {updatedProduct.Id} not found for update.");
                return NotFound(response);
            }
            response.Success = true;
            response.Message = "Product updated successfully.";
            Console.WriteLine($"Product with ID: {updatedProduct.Id} updated successfully.");
            return Ok(response);
        }
        catch (Exception ex)
        {
            // Log the exception (ex) here if needed
            response.Success = false;
            response.Message = "Internal server error while updating the product.";
            Console.WriteLine($"Error updating product with ID: {updatedProduct.Id}. Exception: {ex.Message}");
            return StatusCode(500, response);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ServiceResponse<bool>>> DeleteProduct(int id)
    {
        var response = new ServiceResponse<bool>();
        Console.WriteLine($"Received request to delete product with ID: {id}");
        try
        {
            var result = await _context.Products.DeleteOneAsync(p => p.Id == id);
            if (result.DeletedCount == 0)
            {
                response.Success = false;
                response.Data = false;
                response.Message = "Product not found.";
                Console.WriteLine($"Product with ID: {id} not found for deletion.");
                return NotFound(response);
            }
            response.Success = true;
            response.Data = true;
            response.Message = "Product deleted successfully.";
            Console.WriteLine($"Product with ID: {id} deleted successfully.");
            return Ok(response);
        }
        catch (Exception ex)
        {
            // Log the exception (ex) here if needed
            response.Success = false;
            response.Data = false;
            response.Message = "Internal server error while deleting the product.";
            Console.WriteLine($"Error deleting product with ID: {id}. Exception: {ex.Message}");
            return StatusCode(500, response);
        }
    }
}
