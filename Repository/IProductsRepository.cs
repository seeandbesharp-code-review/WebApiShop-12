using Entities;
namespace Repository
{
    public interface IProductsRepository
    {
        Task<IEnumerable<Product>> GetProducts(int[]? categoryId, decimal maxPrice, decimal minPrice);
        Task<Product> CreateProduct(Product Product);
        Task<Product?> GetProductById(int id);
        Task UpdateProduct(int id, Product Product);
    }
}