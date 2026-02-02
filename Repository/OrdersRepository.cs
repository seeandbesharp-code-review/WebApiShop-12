using Entities;
using Microsoft.EntityFrameworkCore;


namespace Repository
{
    public class OrdersRepository : IOrdersRepository
    {
        WebApiShopContext _webApiShopContext;

        public OrdersRepository(WebApiShopContext webApiShopContext)
        {
            _webApiShopContext = webApiShopContext;
        }

        public async Task<IEnumerable<Order>> GetOrders()
        {
            return await _webApiShopContext.Orders.ToListAsync();
        }

        public async Task<Order?> GetOrderById(int id)
        {
            return await _webApiShopContext.Orders.FindAsync(id);
        }

        public async Task<Order> CreateOrder(Order order)
        {
            await _webApiShopContext.Orders.AddAsync(order);
            await _webApiShopContext.SaveChangesAsync();
            return order;
        }
    }
}
