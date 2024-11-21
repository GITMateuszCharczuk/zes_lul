using MyMauiApp.Models;

namespace MyMauiApp.Services;

public class ProductService : IProductService
{
    private readonly List<Product> _products = new();

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await Task.FromResult(_products);
    }

    public async Task<Product> GetProductByIdAsync(int id)
    {
        return await Task.FromResult(_products.FirstOrDefault(p => p.Id == id));
    }

    public async Task AddProductAsync(Product product)
    {
        product.Id = _products.Count > 0 ? _products.Max(p => p.Id) + 1 : 1;
        _products.Add(product);
        await Task.CompletedTask;
    }

    public async Task UpdateProductAsync(Product product)
    {
        var existingProduct = _products.FirstOrDefault(p => p.Id == product.Id);
        if (existingProduct != null)
        {
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
        }
        await Task.CompletedTask;
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product != null)
        {
            _products.Remove(product);
        }
        await Task.CompletedTask;
    }
}
