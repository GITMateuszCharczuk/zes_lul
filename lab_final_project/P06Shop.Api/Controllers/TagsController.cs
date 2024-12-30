using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using P06Shop.Api.Data;
using P06Shop.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using P06Shop.Api.Models.Dto;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly MongoDbContext _context;

    public TagsController(MongoDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<ServiceResponse<IEnumerable<ProductTag>>>> GetTags()
    {
        var response = new ServiceResponse<IEnumerable<ProductTag>>();
        try
        {
            var tags = await _context.Tags.Find(_ => true).ToListAsync();
            response.Data = tags;
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
    public async Task<ActionResult<ServiceResponse<ProductTag>>> GetTag(int id)
    {
        var response = new ServiceResponse<ProductTag>();
        try
        {
            var tag = await _context.Tags.Find(t => t.Id == id).FirstOrDefaultAsync();
            if (tag == null)
            {
                response.Success = false;
                response.Message = "Tag not found.";
                return NotFound(response);
            }
            response.Data = tag;
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
    public async Task<ActionResult<ServiceResponse<ProductTag>>> CreateTag(CreateTagDTO newTagDto)
    {
        var response = new ServiceResponse<ProductTag>();
        try
        {
            var random = new Random();
            var newTag = new ProductTag
            {
                Id = random.Next(1, 10000),
                Name = newTagDto.Name,
                Products = new List<Product>()
            };

            foreach (var productId in newTagDto.ProductIds)
            {
                var product = await _context.Products.Find(p => p.Id == productId).FirstOrDefaultAsync();
                if (product != null)
                {
                    newTag.Products.Add(product);
                }
            }

            await _context.Tags.InsertOneAsync(newTag);
            response.Data = newTag;
            response.Success = true;
            response.Message = "Tag created successfully.";
            return CreatedAtAction(nameof(GetTag), new { id = newTag.Id }, response);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Internal server error while creating the tag.";
            return StatusCode(500, response);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ServiceResponse<ProductTag>>> UpdateTag(int id, ProductTag updatedTag)
    {
        var response = new ServiceResponse<ProductTag>();
        try
        {
            var result = await _context.Tags.ReplaceOneAsync(t => t.Id == id, updatedTag);
            if (result.MatchedCount == 0)
            {
                response.Success = false;
                response.Message = "Tag not found.";
                return NotFound(response);
            }
            response.Success = true;
            response.Message = "Tag updated successfully.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Internal server error while updating the tag.";
            return StatusCode(500, response);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ServiceResponse<bool>>> DeleteTag(int id)
    {
        var response = new ServiceResponse<bool>();
        try
        {
            var result = await _context.Tags.DeleteOneAsync(t => t.Id == id);
            if (result.DeletedCount == 0)
            {
                response.Success = false;
                response.Data = false;
                response.Message = "Tag not found.";
                return NotFound(response);
            }
            response.Success = true;
            response.Data = true;
            response.Message = "Tag deleted successfully.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Data = false;
            response.Message = "Internal server error while deleting the tag.";
            return StatusCode(500, response);
        }
    }
}