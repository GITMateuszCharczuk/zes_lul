namespace P06Shop.Api.Models
{
    public class ServiceResponse<T>
    {
        public T? Data { get; set; } = default;
        public bool Success { get; set; } = false;
        public string Message { get; set; } = null;
    }
}
