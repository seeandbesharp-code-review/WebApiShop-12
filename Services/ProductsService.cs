using Repository;
using AutoMapper;
using DTOs;
using Entities;
using System.Collections.Generic;
using System.Net.Security;
namespace Services
{
    public class ProductsService : IProductsService
    {
        private readonly IProductsRepository _repository;
        private readonly IMapper _mapper;

        public ProductsService(IProductsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDTO>> GetProducts(int[]? categoryId, decimal maxPrice, decimal minPrice)
        {
            IEnumerable<Product> products = await _repository.GetProducts(categoryId, maxPrice, minPrice);
            return  _mapper.Map<IEnumerable<Product>,IEnumerable<ProductDTO>>(products);
        }

        public async Task<ProductDTO?> GetProductById(int id)
        {
            Product? product = await _repository.GetProductById(id);
            return _mapper.Map<Product, ProductDTO>(product);
        }

        public async Task<ProductDTO?> CreateProduct(ProductDTO product)
        {
            
            Product product1 = _mapper.Map<ProductDTO, Product>(product);
            product1 = await _repository.CreateProduct(product1);
            return _mapper.Map<Product, ProductDTO>(product1);
        }
        
        public async Task UpdateProduct(int id, ProductDTO product)
        {
            Product productEntity = _mapper.Map<ProductDTO,Product>(product);
            await _repository.UpdateProduct(id, productEntity);
        }
    }
}
