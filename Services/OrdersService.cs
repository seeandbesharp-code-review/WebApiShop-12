using Repository;
using AutoMapper;
using DTOs;
using Entities;
using System.Collections.Generic;
using System.Net.Security;
namespace Services
{
    public class OrdersService : IOrdersService
    {
        private readonly IOrdersRepository _repository;
        private readonly IMapper _mapper;

        public OrdersService(IOrdersRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderDTO>> GetOrders()
        {
            IEnumerable<Order> orders = await _repository.GetOrders();
            return  _mapper.Map<IEnumerable<Order>,IEnumerable<OrderDTO>>(orders);
        }

        public async Task<OrderDTO> GetOrderById(int id)
        {
            Order? order = await _repository.GetOrderById(id);
            return _mapper.Map<Order, OrderDTO>(order);
        }

        public async Task<OrderDTO> CreateOrder(OrderDTO order)
        {
            Order order1 = _mapper.Map<OrderDTO, Order>(order);
            order1 = await _repository.CreateOrder(order1);
            return _mapper.Map<Order, OrderDTO>(order1);
        }
    }
}
