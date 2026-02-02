using Entities;
using DTOs;

namespace Services
{
    public interface ICategoriesService
    {
        Task<IEnumerable<CategoryDTO>> GetCategories();
        Task<CategoryDTO?> CreateCategory(CategoryDTO category);
    }
}