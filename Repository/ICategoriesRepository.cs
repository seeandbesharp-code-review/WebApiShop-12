using Entities;
namespace Repository
{
    public interface ICategoriesRepository
    {
        Task<IEnumerable<Category>> GetCategories();
        Task<Category?> GetCategoryById(int id);
        Task<Category> CreateCategory(Category category);
    }
}
