using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMauiApp.Models;
using MyMauiApp.Services;

namespace MyMauiApp.ViewModels;

public partial class ProductsViewModel : ObservableObject
{
    private readonly IProductService _productService;

    public ObservableCollection<Product> Products { get; } = new();
    
    [ObservableProperty]
    private Product selectedProduct;

    public ProductsViewModel(IProductService productService)
    {
        _productService = productService;
        LoadProductsCommand = new AsyncRelayCommand(LoadProductsAsync);
        AddProductCommand = new AsyncRelayCommand(AddProductAsync);
        UpdateProductCommand = new AsyncRelayCommand(UpdateProductAsync);
        DeleteProductCommand = new AsyncRelayCommand(DeleteProductAsync);
    }
    public ProductsViewModel()
    {
    }

    public IAsyncRelayCommand LoadProductsCommand { get; }
    public IAsyncRelayCommand AddProductCommand { get; }
    public IAsyncRelayCommand UpdateProductCommand { get; }
    public IAsyncRelayCommand DeleteProductCommand { get; }

    private async Task LoadProductsAsync()
    {
        Products.Clear();
        var products = await _productService.GetAllProductsAsync();
        foreach (var product in products)
            Products.Add(product);
    }

    private async Task AddProductAsync()
    {
        var newProduct = new Product { Name = "New Product", Description = "Description", Price = 0 };
        await _productService.AddProductAsync(newProduct);
        await LoadProductsAsync();
    }

    private async Task UpdateProductAsync()
    {
        if (SelectedProduct != null)
        {
            await _productService.UpdateProductAsync(SelectedProduct);
            await LoadProductsAsync();
        }
    }

    private async Task DeleteProductAsync()
    {
        if (SelectedProduct != null)
        {
            await _productService.DeleteProductAsync(SelectedProduct.Id);
            await LoadProductsAsync();
        }
    }
}
