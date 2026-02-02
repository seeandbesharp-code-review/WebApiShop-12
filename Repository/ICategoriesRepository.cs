using Entities;
namespace Repository
{
    public interface ICategoriesRepository
    {
        Task<IEnumerable<Category>> GetCategories();
        Task<Category> CreateCategory(Category category);
    }
}
