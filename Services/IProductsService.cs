using Entities;
using DTOs;

namespace Services
{
    public interface IProductsService
    {
        Task<PageResponseDTO> GetProducts(int[]? categoryId, decimal maxPrice, decimal minPrice, string desc, int position, int skip);
        Task<ProductDTO?> CreateProduct(ProductDTO product);
        Task<ProductDTO?> GetProductById(int id);
        Task UpdateProduct(int id, ProductDTO product);
    }
}