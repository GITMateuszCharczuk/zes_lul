using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using P06Shop.Api.Data;
using P06Shop.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using P06Shop.Api.Models.Dto;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly MongoDbContext _context;

    public CategoriesController(MongoDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<ServiceResponse<IEnumerable<Category>>>> GetCategories()
    {
        var response = new ServiceResponse<IEnumerable<Category>>();
        try
        {
            var categories = await _context.Categories.Find(_ => true).ToListAsync();
            response.Data = categories;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Internal server error.";
            return StatusCode(500, response);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResponse<Category>>> GetCategory(int id)
    {
        var response = new ServiceResponse<Category>();
        try
        {
            var category = await _context.Categories.Find(c => c.Id == id).FirstOrDefaultAsync();
            if (category == null)
            {
                response.Success = false;
                response.Message = "Category not found.";
                return NotFound(response);
            }
            response.Data = category;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Internal server error.";
            return StatusCode(500, response);
        }
    }

    [HttpPost]
    public async Task<ActionResult<ServiceResponse<Category>>> CreateCategory(CreateCategoryDTO newCategoryDto)
    {
        var response = new ServiceResponse<Category>();
        try
        {
            var random = new Random();
            var newCategory = new Category
            {
                Id = random.Next(1, 10000),
                Name = newCategoryDto.Name,
                Products = new List<Product>()
            };

            foreach (var productId in newCategoryDto.ProductIds)
            {
                var product = await _context.Products.Find(p => p.Id == productId).FirstOrDefaultAsync();
                if (product != null)
                {
                    newCategory.Products.Add(product);
                }
            }

            await _context.Categories.InsertOneAsync(newCategory);
            response.Data = newCategory;
            response.Success = true;
            response.Message = "Category created successfully.";
            return CreatedAtAction(nameof(GetCategory), new { id = newCategory.Id }, response);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Internal server error while creating the category.";
            return StatusCode(500, response);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ServiceResponse<Category>>> UpdateCategory(int id, Category updatedCategory)
    {
        var response = new ServiceResponse<Category>();
        try
        {
            var result = await _context.Categories.ReplaceOneAsync(c => c.Id == id, updatedCategory);
            if (result.MatchedCount == 0)
            {
                response.Success = false;
                response.Message = "Category not found.";
                return NotFound(response);
            }
            response.Success = true;
            response.Message = "Category updated successfully.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Internal server error while updating the category.";
            return StatusCode(500, response);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ServiceResponse<bool>>> DeleteCategory(int id)
    {
        var response = new ServiceResponse<bool>();
        try
        {
            var result = await _context.Categories.DeleteOneAsync(c => c.Id == id);
            if (result.DeletedCount == 0)
            {
                response.Success = false;
                response.Data = false;
                response.Message = "Category not found.";
                return NotFound(response);
            }
            response.Success = true;
            response.Data = true;
            response.Message = "Category deleted successfully.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Data = false;
            response.Message = "Internal server error while deleting the category.";
            return StatusCode(500, response);
        }
    }
}