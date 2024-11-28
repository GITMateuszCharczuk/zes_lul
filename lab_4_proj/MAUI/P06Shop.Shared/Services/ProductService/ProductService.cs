using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using P06Shop.Shared;
using P06Shop.Shared.Confguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace P06Shop.Shared.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        private readonly AppSettings _appSettings;

        public ProductService(HttpClient httpClient, IOptions<AppSettings> appSettings)
        {
            _httpClient = httpClient;
            _appSettings = appSettings.Value;
        }

        public async Task<ServiceReponse<Product>> CreateProductAsync(Product newProduct)
        {
            var response = await _httpClient.PostAsJsonAsync(_appSettings.ProductEndpoint.CreateProduct, newProduct);
            var result = await response.Content.ReadFromJsonAsync<ServiceReponse<Product>>();
            return result;
        }

        public async Task<ServiceReponse<bool>> DeleteProductAsync(int id)
        {
            // jeżeli uzyjemy / na początku to będzie to adres bezwzględny
            // czyli scieżka bedzie wyglądać tak: https://localhost:5001/1
            // zacznyamy od roota
            var response = await _httpClient.DeleteAsync($"{id}");
            var result = await response.Content.ReadFromJsonAsync<ServiceReponse<bool>>();
            return result;
        }

        public async Task<ServiceReponse<Product>> GetProductAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{id}");
            var result = await response.Content.ReadFromJsonAsync<ServiceReponse<Product>>();
            return result;
        }

        public async Task<ServiceReponse<List<Product>>> GetProductsAsync()
        {
            var response = await _httpClient.GetAsync(_appSettings.ProductEndpoint.GetProducts);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ServiceReponse<List<Product>>>(json);
            return result;
        }

        public async Task<ServiceReponse<List<Product>>> SearchProductsAsync(string text, int page, int pageSize)
        {
            try
            {
                string searchUrl = _appSettings.ProductEndpoint.SearchProducts;
                string query = string.IsNullOrEmpty(text) ? "" : $"{text}/";
                query += $"{page}/{pageSize}";
                var response = await _httpClient.GetAsync(searchUrl +"/" + query);
                if (!response.IsSuccessStatusCode)
                {
                    return new ServiceReponse<List<Product>>
                    {
                        Success = false,
                        Message = response.ReasonPhrase
                    };
                }

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ServiceReponse<List<Product>>>(json)
                    ?? new ServiceReponse<List<Product>>() { Success = false, Message = "Deserialization failed" };

                result.Success = result.Success && result.Data != null;

                return result;
            }
            catch (JsonException)
            {
                return new ServiceReponse<List<Product>>
                {
                    Success = false,
                    Message = "Deserialization failed"
                };
                
            }catch(Exception ex)
            {
                return new ServiceReponse<List<Product>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }

        }

        public async Task<ServiceReponse<Product>> UpdateProductAsync(Product updatedProduct)
        {
            var response = await _httpClient.PutAsJsonAsync(_appSettings.ProductEndpoint.UpdateProduct, updatedProduct);
            var result = await response.Content.ReadFromJsonAsync<ServiceReponse<Product>>();
            return result;
        }
    }
}
