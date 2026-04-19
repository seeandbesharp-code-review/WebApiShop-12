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
        readonly IProductsRepository _repository;
        readonly IMapper _mapper;

        public async Task<PageResponseDTO> GetProducts(int[] categoryId, decimal maxPrice, decimal minPrice, string desc, int position, int skip)
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
        
    }
}
