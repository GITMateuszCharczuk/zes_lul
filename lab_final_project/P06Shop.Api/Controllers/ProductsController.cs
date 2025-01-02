using Microsoft.AspNetCore.Mvc;
using P06Shop.Api.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using P06Shop.Api.Services;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;

    public ProductsController(ProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetProductDTO>>> GetProducts()
    {
        var result = await _productService.GetProducts();
        if (!result.Success)
            return NotFound(result.ErrorMessage);

        return Ok(result.Products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetProductDTO>> GetProduct(string id)
    {
        var result = await _productService.GetProduct(id);
        if (!result.Success)
            return NotFound(result.ErrorMessage);

        return Ok(result.Product);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GetProductDTO>> CreateProduct(CreateProductDTO productDto)
    {
        var result = await _productService.CreateProduct(productDto);
        if (!result.Success)
            return StatusCode(500, result.ErrorMessage);

        return CreatedAtAction(nameof(GetProduct), new { id = result.Product!.Id }, result.Product);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GetProductDTO>> UpdateProduct(string id, UpdateProductDTO updateDto)
    {
        var result = await _productService.UpdateProduct(id, updateDto);
        if (!result.Success)
            return NotFound(result.ErrorMessage);

        return Ok(result.Product);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProduct(string id)
    {
        var result = await _productService.DeleteProduct(id);
        if (!result.Success)
            return NotFound(result.ErrorMessage);

        return NoContent();
    }
}
