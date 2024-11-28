using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P06Shop.Shared.Services.ProductService
{
    public interface IProductService
    {
        Task<ServiceReponse<List<Product>>> GetProductsAsync();
        Task<ServiceReponse<Product>> CreateProductAsync(Product newProduct);
        Task<ServiceReponse<bool>> DeleteProductAsync(int id);
        Task<ServiceReponse<Product>> UpdateProductAsync(Product updatedProduct);
        Task<ServiceReponse<Product>> GetProductAsync(int id);

        Task<ServiceReponse<List<Product>>> SearchProductsAsync(string text, int page, int pageSize);
     
    }
}
