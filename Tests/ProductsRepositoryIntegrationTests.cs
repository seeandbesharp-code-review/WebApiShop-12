using Entities;
using Repository;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests
{
    public class ProductsRepositoryIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly WebApiShopContext _dbContext;
        private readonly ProductsRepository _productsRepository;

        public ProductsRepositoryIntegrationTests(DatabaseFixture databaseFixture)
        {
            _dbContext = databaseFixture.Context;
            _productsRepository = new ProductsRepository(_dbContext);

            // ניקוי נתונים לפני כל טסט כדי להבטיח סביבה נקייה (אופציונלי)
            _dbContext.Products.RemoveRange(_dbContext.Products);
            _dbContext.Categories.RemoveRange(_dbContext.Categories);
            _dbContext.SaveChanges();
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }

        [Fact]
        public async Task GetProducts_FiltersByDescription_ReturnsCorrectProducts()
        {
            // Arrange
            var category = new Category { CategoryName = "Tech" };
            var product1 = new Product { ProductName = "Laptop", Description = "Gaming Laptop", Price = 500, Category = category };
            var product2 = new Product { ProductName = "Mouse", Description = "Office Mouse", Price = 50, Category = category };

            await _dbContext.Products.AddRangeAsync(product1, product2);
            await _dbContext.SaveChangesAsync();

            // Act
            // חיפוש מוצרים שמכילים את המילה "Gaming" בתיאור
            var (products, total) = await _productsRepository.GetProducts(new int[] { }, 0, 0, "Gaming", 1, 10);

            // Assert
            Assert.Equal(1, total);
            Assert.Contains(products, p => p.ProductName == "Laptop");
        }

        [Fact]
        public async Task CreateProduct_AddsProductToRealDatabase()
        {
            // Arrange
            var category = new Category { CategoryName = "Books" };
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            var newProduct = new Product
            {
                ProductName = "C# Guide",
                Description = "Learn Programming",
                Price = 100,
                CategoryId = category.CategoryId
            };

            // Act
            var result = await _productsRepository.CreateProduct(newProduct);

            // Assert
            var productInDb = await _dbContext.Products.FindAsync(result.ProductId);
            Assert.NotNull(productInDb);
            Assert.Equal("C# Guide", productInDb.ProductName);
        }

        [Fact]
        public async Task GetProducts_FiltersByPriceRange_ReturnsFilteredResults()
        {
            // Arrange
            var category = new Category { CategoryName = "Electronics" };
            _dbContext.Products.AddRange(
                new Product { ProductName = "Cheap", Price = 10, Description = "Item 1", Category = category },
                new Product { ProductName = "Mid", Price = 50, Description = "Item 2", Category = category },
                new Product { ProductName = "Expensive", Price = 100, Description = "Item 3", Category = category }
            );
            await _dbContext.SaveChangesAsync();

            // Act
            // טווח מחירים בין 40 ל-60
            var (products, total) = await _productsRepository.GetProducts(new int[] { }, 60, 40, "", 1, 10);

            // Assert
            Assert.Single(products);
            Assert.Equal("Mid", products.First().ProductName);
        }

        [Fact]
        public async Task GetProducts_FilterByPriceRange_ReturnsOnlyMatchingProducts()
        {
            // Arrange
            var category = new Category { CategoryName = "Home" };
            _dbContext.Products.AddRange(
                new Product { ProductName = "Cheap Item", Price = 20, Description = "Desc", Category = category },
                new Product { ProductName = "Mid Item", Price = 100, Description = "Desc", Category = category },
                new Product { ProductName = "Expensive Item", Price = 500, Description = "Desc", Category = category }
            );
            await _dbContext.SaveChangesAsync();

            // Act 
            var (products, total) = await _productsRepository.GetProducts(new int[] { }, 150, 50, "", 1, 10);

            // Assert
            Assert.Equal(1, total);
            Assert.Equal("Mid Item", products.First().ProductName);
        }

        [Fact]
        public async Task UpdateProduct_UpdatesExistingProductInDb()
        {
            // Arrange
            var category = new Category { CategoryName = "Outdoors" };
            var product = new Product { ProductName = "Tent", Price = 300, Description = "Old Desc", Category = category };
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            product.ProductName = "Updated Tent";
            product.Price = 350;

            // Act
            await _productsRepository.UpdateProduct(product.ProductId, product);

            // Assert
            var updatedProduct = await _dbContext.Products.FindAsync(product.ProductId);
            Assert.Equal("Updated Tent", updatedProduct.ProductName);
            Assert.Equal(350, updatedProduct.Price);
        }
    }
}