using Entities;
using DTOs;

namespace Services
{
    public interface IOrdersService
    {
        Task<IEnumerable<OrderDTO>> GetOrders();
        Task<OrderDTO> CreateOrder(OrderDTO order);
        Task<OrderDTO> GetOrderById(int id);
    }
}