using System.Collections.Generic;
using Entities;
using Microsoft.EntityFrameworkCore;


namespace Repository
{
    public class ProductsRepository : IProductsRepository
    {
        readonly WebApiShopContext _webApiShopContext;

        public ProductsRepository(WebApiShopContext webApiShopContext)
        {
            _webApiShopContext = webApiShopContext;
        }

        public async Task<(IEnumerable<Product>,int)> GetProducts(int[] categoryId, decimal maxPrice, decimal minPrice, string desc, int position, int skip)
        {
            var query = _webApiShopContext.Products.Where(product=>
            ((desc == "" )? (true) : (product.Description.Contains(desc))) &&
            ((minPrice == 0) ? (true) : (product.Price >= minPrice)) &&
            ((maxPrice == 0) ? (true) : (product.Price <= maxPrice)) &&
            ((categoryId.Length == 0) ? (true) : (categoryId.Contains(product.ProductId))))
            .OrderBy(product=> product.Price);

            var total = await query.CountAsync();
            var products = await query.Skip((position-1)*skip)
                .Take(skip).Include(product=> product.Category).ToListAsync();
            return (products, total);
        }

        public async Task<Product?> GetProductById(int id)
        {
            return await _webApiShopContext.Products.FindAsync(id);
        }
        public async Task UpdateProduct(int id, Product product)
        {
            _webApiShopContext.Products.Update(product);
            await _webApiShopContext.SaveChangesAsync();
        }

        public async Task<Product> CreateProduct(Product product)
        {
            await _webApiShopContext.Products.AddAsync(product);
            await _webApiShopContext.SaveChangesAsync();
            return product;
        }

    }
}
