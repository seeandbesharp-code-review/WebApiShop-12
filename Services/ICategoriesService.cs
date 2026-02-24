using Entities;
using DTOs;

namespace Services
{
    public interface ICategoriesService
    {
        Task<IEnumerable<CategoryDTO>> GetCategories();
        Task<CategoryDTO?> GetCategoryById(int id);
        Task<CategoryDTO?> CreateCategory(CategoryDTO category);
    }
}