using Entities;
using DTOs;

namespace Services
{
    public interface IProductsService
    {
        Task<IEnumerable<ProductDTO>> GetProducts(int[]? categoryId, decimal maxPrice, decimal minPrice);
        Task<ProductDTO?> CreateProduct(ProductDTO product);
        Task<ProductDTO?> GetProductById(int id);
        Task UpdateProduct(int id, ProductDTO product);
    }
}