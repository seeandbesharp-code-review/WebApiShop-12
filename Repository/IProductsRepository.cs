using Entities;
namespace Repository
{
    public interface IProductsRepository
    {
        Task<(IEnumerable<Product>,int)> GetProducts(int[]? categoryId, decimal maxPrice, decimal minPrice, string desc, int position, int skip);
        Task<Product> CreateProduct(Product Product);
        Task<Product?> GetProductById(int id);
        Task UpdateProduct(int id, Product Product);
    }
}