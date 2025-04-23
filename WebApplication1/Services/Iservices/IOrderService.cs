using WebApplication1.Models;

namespace WebApplication1.Services.Iservices
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order> GetOrderByIdAsync(int id);
        Task AddOrderAsync(Order order);
        Task UpdateOrderAsync(int id, Order order);
        Task DeleteOrderAsync(int id);
        Task ChangeOrderStatusAsync(int id, OrderStatus status);
    }

}
