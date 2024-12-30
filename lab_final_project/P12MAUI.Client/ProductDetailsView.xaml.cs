using P12MAUI.Client.ViewModels;

namespace P12MAUI.Client;

public partial class ProductDetailsView : ContentPage
{
	public ProductDetailsView(ProductDetailsViewModel productDetailsViewModel)
	{
		BindingContext = productDetailsViewModel;
		InitializeComponent();
	}
}