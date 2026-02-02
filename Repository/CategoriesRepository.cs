using Entities;
using Microsoft.EntityFrameworkCore;


namespace Repository
{
    public class CategoriesRepository : ICategoriesRepository
    {
        WebApiShopContext _webApiShopContext;

        public CategoriesRepository(WebApiShopContext webApiShopContext)
        {
            _webApiShopContext = webApiShopContext;
        }

        public async Task<IEnumerable<Category>> GetCategories()
        {
            return await _webApiShopContext.Categories.ToListAsync();
        }

        

        public async Task<Category> CreateCategory(Category category)
        {
            await _webApiShopContext.Categories.AddAsync(category);
            await _webApiShopContext.SaveChangesAsync();
            return category;
        }

    }
}
