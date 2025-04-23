using WebApplication1.Models;
using WebApplication1.Repository.IRepository;
using WebApplication1.Services.Iservices;

namespace WebApplication1.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrder _order;

        public OrderService(IOrder order)
        {
            _order = order;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await Task.FromResult(_order.Get());
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            var order = _order.GetOne(expression: e => e.Id == id);
            return await Task.FromResult(order);
        }

        public async Task AddOrderAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            _order.Create(order);
            _order.Commit();
            await Task.CompletedTask;
        }

        public async Task UpdateOrderAsync(int id, Order updatedOrder)
        {
            var existingOrder = _order.GetOne(expression: e => e.Id == id);
            if (existingOrder == null)
                throw new KeyNotFoundException("Order not found.");

            existingOrder.Count = updatedOrder.Count;
            existingOrder.Date = updatedOrder.Date;
            existingOrder.ProductID = updatedOrder.ProductID;
            existingOrder.Status = updatedOrder.Status;

            _order.Edit(existingOrder);
            _order.Commit();

            await Task.CompletedTask;
        }

        public async Task DeleteOrderAsync(int id)
        {
            var order = _order.GetOne(expression: e => e.Id == id);
            if (order == null)
                throw new KeyNotFoundException("Order not found.");

            _order.Delete(order);
            _order.Commit();

            await Task.CompletedTask;
        }
        public async Task ChangeOrderStatusAsync(int id, OrderStatus status)
        {
            var existingOrder = _order.GetOne(expression: e => e.Id == id);
            if (existingOrder == null)
                throw new KeyNotFoundException("Order not found.");

            existingOrder.Status = status;  // Update the order status

            _order.Edit(existingOrder);
             _order.Commit();  // Commit the changes to the repository

            await Task.CompletedTask;
        }
    }
}
