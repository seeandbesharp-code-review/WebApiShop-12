using System.Collections.Generic;
using Entities;
using Microsoft.EntityFrameworkCore;


namespace Repository
{
    public class ProductsRepository : IProductsRepository
    {
        WebApiShopContext _webApiShopContext;

        public ProductsRepository(WebApiShopContext webApiShopContext)
        {
            _webApiShopContext = webApiShopContext;
        }

        public async Task<IEnumerable<Product>> GetProducts(int[]? categoryId, decimal maxPrice, decimal minPrice)
        {
            var query = _webApiShopContext.Products.Where(product=>
            (minPrice == 0)? (true) : (product.Price >= minPrice) &&
            (maxPrice == 0) ? (true) : (product.Price <= maxPrice) &&
            (categoryId.Length == 0) ? (true) : (categoryId.Contains(product.ProductId)))
            .OrderBy(product=> product.Price);

            var total = await query.CountAsync();

            return await query.ToListAsync();
        }

        public async Task<Product?> GetProductById(int id)
        {
            return await _webApiShopContext.Products.FindAsync(id);
        }

        public async Task<Product> CreateProduct(Product product)
        {
            await _webApiShopContext.Products.AddAsync(product);
            await _webApiShopContext.SaveChangesAsync();
            return product;
        }

        public async Task UpdateProduct(int id, Product product)
        {
            _webApiShopContext.Products.Update(product);
            await _webApiShopContext.SaveChangesAsync();
        }

    }
}
