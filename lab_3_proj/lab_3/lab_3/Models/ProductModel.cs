namespace lab_3.Models;

public class ProductModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Category { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
}