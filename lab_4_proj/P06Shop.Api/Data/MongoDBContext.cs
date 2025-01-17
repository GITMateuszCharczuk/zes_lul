using MongoDB.Driver;
using P06Shop.Api.Models;

namespace P06Shop.Api.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<Product> Products => _database.GetCollection<Product>("Products");
        public IMongoCollection<ProductDetail> ProductDetails => _database.GetCollection<ProductDetail>("ProductDetails");
        public IMongoCollection<Category> Categories => _database.GetCollection<Category>("Categories");
        public IMongoCollection<ProductTag> Tags => _database.GetCollection<ProductTag>("Tags");
    }
}
