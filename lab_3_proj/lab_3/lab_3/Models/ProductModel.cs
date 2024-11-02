using System.ComponentModel.DataAnnotations;

namespace lab_3.Models;

public class ProductModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Product name is required.")]
    [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Category is required.")]
    [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters.")]
    public string? Category { get; set; }

    [Range(0.01, 10000.00, ErrorMessage = "Price must be between 0.01 and 10,000.00.")]
    public decimal Price { get; set; }

    public bool IsAvailable { get; set; }
}