using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using P06Shop.Api.Data;
using P06Shop.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using P06Shop.Api.Models.Dto;

[ApiController]
[Route("api/[controller]")]
public class ProductDetailsController : ControllerBase
{
    private readonly MongoDbContext _context;

    public ProductDetailsController(MongoDbContext context)
    {
        _context = context;
    }

    [HttpGet("{productId}")]
    public async Task<ActionResult<ServiceResponse<ProductDetail>>> GetProductDetail(int productId)
    {
        var response = new ServiceResponse<ProductDetail>();
        try
        {
            var detail = await _context.ProductDetails.Find(d => d.ProductId == productId).FirstOrDefaultAsync();
            if (detail == null)
            {
                response.Success = false;
                response.Message = "Product detail not found.";
                return NotFound(response);
            }
            response.Data = detail;
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

    [HttpGet]
    public async Task<ActionResult<ServiceResponse<IEnumerable<ProductDetail>>>> GetAllProductDetails()
    {
        var response = new ServiceResponse<IEnumerable<ProductDetail>>();
        try
        {
            var details = await _context.ProductDetails.Find(_ => true).ToListAsync();
            response.Data = details;
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
    public async Task<ActionResult<ServiceResponse<ProductDetail>>> CreateProductDetail(CreateProductDetailDTO newDetailDto)
    {
        var response = new ServiceResponse<ProductDetail>();
        try
        {
            var random = new Random();
            var newDetail = new ProductDetail
            {
                Id = random.Next(1, 10000),
                ProductId = newDetailDto.ProductId,
                Specifications = newDetailDto.Specifications,
                Warranty = newDetailDto.Warranty
            };

            await _context.ProductDetails.InsertOneAsync(newDetail);
            response.Data = newDetail;
            response.Success = true;
            response.Message = "Product detail created successfully.";
            return CreatedAtAction(nameof(GetProductDetail), new { productId = newDetail.ProductId }, response);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Internal server error while creating the product detail.";
            return StatusCode(500, response);
        }
    }

    [HttpPut("{productId}")]
    public async Task<ActionResult<ServiceResponse<ProductDetail>>> UpdateProductDetail(int productId, ProductDetail updatedDetail)
    {
        var response = new ServiceResponse<ProductDetail>();
        try
        {
            var result = await _context.ProductDetails.ReplaceOneAsync(d => d.ProductId == productId, updatedDetail);
            if (result.MatchedCount == 0)
            {
                response.Success = false;
                response.Message = "Product detail not found.";
                return NotFound(response);
            }
            response.Success = true;
            response.Message = "Product detail updated successfully.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Internal server error while updating the product detail.";
            return StatusCode(500, response);
        }
    }

    [HttpDelete("{productId}")]
    public async Task<ActionResult<ServiceResponse<bool>>> DeleteProductDetail(int productId)
    {
        var response = new ServiceResponse<bool>();
        try
        {
            var result = await _context.ProductDetails.DeleteOneAsync(d => d.ProductId == productId);
            if (result.DeletedCount == 0)
            {
                response.Success = false;
                response.Data = false;
                response.Message = "Product detail not found.";
                return NotFound(response);
            }
            response.Success = true;
            response.Data = true;
            response.Message = "Product detail deleted successfully.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Data = false;
            response.Message = "Internal server error while deleting the product detail.";
            return StatusCode(500, response);
        }
    }
}