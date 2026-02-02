using Entities;
namespace Repository
{
    public interface IOrdersRepository
    {
        Task<IEnumerable<Order>> GetOrders();
        Task<Order> CreateOrder(Order order);
        Task<Order?> GetOrderById(int id);
        
    }
}