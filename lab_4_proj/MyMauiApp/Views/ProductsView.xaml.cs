using MyMauiApp.ViewModels;

namespace MyMauiApp.Views;

public partial class ProductsView : ContentPage
{
    public ProductsView(ViewModels.ProductsViewModel viewModel)
    {
        InitializeComponent();
        //BindingContext = viewModel;
        BindingContext = new ProductsViewModel();
    }
}

