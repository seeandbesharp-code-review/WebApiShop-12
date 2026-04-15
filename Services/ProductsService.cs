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
        public ProductsService(IProductsRepository repository, IMapper mapper)
        {
            this._repository = repository;
            _mapper = mapper;
        }
        IProductsRepository _repository;
        IMapper _mapper;

        public async Task<PageResponseDTO> GetProducts(int[]? categoryId, decimal maxPrice, decimal minPrice, string desc, int position, int skip)
        {
            var (products, total) = await _repository.GetProducts(categoryId, maxPrice, minPrice, desc, position, skip);

            var data = _mapper.Map<IEnumerable<Product>, IEnumerable<ProductDTO>>(products);
            int numOfPages = total / skip;
            if (total % skip != 0)
                numOfPages++;
            var pageResponse = new PageResponseDTO(
                data,
                total,
                position,
                skip,
                position < numOfPages,
                position > 1,
                numOfPages
            );
            return pageResponse;
        }

        public async Task<ProductDTO?> GetProductById(int id)
        {
            Product? product = await _repository.GetProductById(id);
            return _mapper.Map<Product, ProductDTO>(product);
        }

        public async Task<ProductDTO?> CreateProduct(ProductDTO Product)
        {
            
            Product product1 = _mapper.Map<ProductDTO, Product>(Product);
            product1 = await _repository.CreateProduct(product1);
            return _mapper.Map<Product, ProductDTO>(product1);
        }
        
        public async Task UpdateProduct(int id, ProductDTO Product)
        {
            Product Product1 = _mapper.Map<ProductDTO,Product>(Product);
            await _repository.UpdateProduct(id, Product1);
        }
    }
}
