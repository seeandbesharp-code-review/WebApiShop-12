using Repository;
using AutoMapper;
using DTOs;
using Entities;
using System.Collections.Generic;
using System.Net.Security;
namespace Services
{
    public class CategoriesService : ICategoriesService
    {
        private readonly ICategoriesRepository _repository;
        private readonly IMapper _mapper;

        public CategoriesService(ICategoriesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDTO>> GetCategories()
        {
            IEnumerable<Category> categories = await _repository.GetCategories();
            return  _mapper.Map<IEnumerable<Category>,IEnumerable<CategoryDTO>>(categories);
        }

        

        public async Task<CategoryDTO?> CreateCategory(CategoryDTO category)
        {
            Category category1 = _mapper.Map<CategoryDTO, Category>(category);
            category1 = await _repository.CreateCategory(category1);
            return _mapper.Map<Category, CategoryDTO>(category1);
        }
        
        
    }
}
