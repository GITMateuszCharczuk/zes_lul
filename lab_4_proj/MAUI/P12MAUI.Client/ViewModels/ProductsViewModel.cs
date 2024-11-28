using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using P06Shop.Shared;
using P06Shop.Shared.MessageBox;
using P06Shop.Shared.Services.ProductService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P12MAUI.Client.ViewModels
{
    public partial class ProductsViewModel : ObservableObject
    {
        private readonly IProductService _productService;
        private readonly IMessageDialogService _messageDialogService;
        private readonly IConnectivity _connectivity;
    

        [ObservableProperty]
        private ObservableCollection<Product> _products;


        [ObservableProperty]
        private Product _selectedProduct;

   

        public ProductsViewModel(IProductService productService, IMessageDialogService messageDialogService, IConnectivity connectivity)
        {
            _productService = productService;
            _messageDialogService = messageDialogService;
            _connectivity = connectivity;

            GetProductsAsync();
        }

        public async Task GetProductsAsync()
        {
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                _messageDialogService.ShowMessage("Internet not avaible!");
                return;
            }


            var result = await _productService.GetProductsAsync();
            if (result.Success)
            {
                Products = new ObservableCollection<Product>(result.Data);
            }
        }

       

        [RelayCommand]
        public async Task ShowDetails(Product product)
        {
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                _messageDialogService.ShowMessage("Internet not avaible!");
                return;
            }

            SelectedProduct = product;
      


            await Shell.Current.GoToAsync(nameof(ProductDetailsView), true, new Dictionary<string, object>
            {
                {"Product",product },
                {nameof(ProductsViewModel), this }
            });
        }

        [RelayCommand]
        public async Task New()
        {
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                _messageDialogService.ShowMessage("Internet not avaible!");
                return;
            }

            SelectedProduct = new Product();

            await Shell.Current.GoToAsync(nameof(ProductDetailsView), true, new Dictionary<string, object>
            {
                {"Product",SelectedProduct },
                {nameof(ProductsViewModel), this }
            });
        }

      
    }
}


 