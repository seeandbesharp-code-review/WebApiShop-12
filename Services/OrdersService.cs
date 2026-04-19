using Repository;
using AutoMapper;
using DTOs;
using Entities;
using System.Collections.Generic;
using System.Net.Security;
using Microsoft.Extensions.Logging;
namespace Services
{
    public class OrdersService : IOrdersService
    {
        public OrdersService(IOrdersRepository repository, IMapper mapper, IProductsService productsService, ILogger<OrdersService> logger)
        {
            this._repository = repository;
            _mapper = mapper;
            _productsService = productsService;
            _logger = logger;
        }
        readonly IOrdersRepository _repository;
        readonly IMapper _mapper;
        readonly IProductsService _productsService;
        readonly ILogger<OrdersService> _logger;

        public async Task<IEnumerable<OrderDTO>> GetOrders()
        {
            IEnumerable<Order> Orders = await _repository.GetOrders();
            return  _mapper.Map<IEnumerable<Order>,IEnumerable<OrderDTO>>(Orders);
        }

        public async Task<OrderDTO> GetOrderById(int id)
        {
            Order? order = await _repository.GetOrderById(id);
            return _mapper.Map<Order, OrderDTO>(order);
        }

        public async Task<OrderDTO> CreateOrder(OrderDTO order)
        {
            Order order1 = _mapper.Map<OrderDTO, Order>(order);
            await orderSumValidation(order1);
            order1 = await _repository.CreateOrder(order1);
            return _mapper.Map<Order, OrderDTO>(order1);
        }

        public async Task<int> orderSumValidation(Order order)
        {
            decimal _totalSum = 0;
            foreach (var orderItem in order.OrderItems)
            {
                var product = await _productsService.GetProductById(orderItem.ProductId);
                if (product != null) {
                    _totalSum += product.Price * orderItem.Quantity;
                }
            }
            if(_totalSum != order.OrderSum)
            {
                order.OrderSum = _totalSum;
                _logger.LogWarning("user id:" + order.UserId + "tried to place order with unmatched sum");
            }
            return 1;
        }
    }
}
