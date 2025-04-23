using WebApplication1.Models;
using WebApplication1.Repository.IRepository;
using WebApplication1.Services.Iservices;
using Microsoft.Extensions.Logging;

namespace WebApplication1.Services
{
    public class CartService : ICartService
    {
        private readonly ICart _cart;
        private readonly ILogger<CartService> _logger;

        public CartService(ICart cart, ILogger<CartService> logger)
        {
            _cart = cart;
            _logger = logger;
        }

        public IEnumerable<Cart> GetUserCart(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException(nameof(userId));
            return _cart.Get(expression: e => e.UserID == userId);
        }

        public Cart GetCartItem(int cartId)
        {
            return _cart.GetOne(expression: e => e.Id == cartId);
        }

        public void AddToCart(Cart cartItem)
        {
            if (cartItem == null) throw new ArgumentNullException(nameof(cartItem));

            var existing = _cart.GetOne(expression: e => e.UserID == cartItem.UserID && e.ProductId == cartItem.ProductId);
            if (existing != null)
            {
                existing.Count += cartItem.Count;
                _cart.Edit(existing);
                _logger.LogInformation("Updated cart item (ProductId: {ProductId}) for UserID {UserID}", cartItem.ProductId, cartItem.UserID);
            }
            else
            {
                _cart.Create(cartItem);
                _logger.LogInformation("Created new cart item (ProductId: {ProductId}) for UserID {UserID}", cartItem.ProductId, cartItem.UserID);
            }

            _cart.Commit();
        }

        public void UpdateCartItem(int id, Cart updatedItem)
        {
            if (updatedItem == null) throw new ArgumentNullException(nameof(updatedItem));

            var existing = _cart.GetOne(expression: e => e.Id == id);
            if (existing != null)
            {
                existing.Count = updatedItem.Count;
                existing.ProductId = updatedItem.ProductId;
                _cart.Edit(existing);
                _cart.Commit();

                _logger.LogInformation("Updated cart item ID {Id} for UserID {UserID}", id, updatedItem.UserID);
            }
        }

        public bool RemoveFromCart(int id)
        {
            var cartItem = _cart.GetOne(expression: e => e.Id == id);
            if (cartItem != null)
            {
                _cart.Delete(cartItem);
                _cart.Commit();
                _logger.LogInformation("Removed cart item ID {Id}", id);
                return true;
            }

            return false;
        }

        public void ClearCart(string userId)
        {
            var cartItems = _cart.Get(expression: e => e.UserID == userId).ToList();
            foreach (var item in cartItems)
            {
                _cart.Delete(item);
            }
            _cart.Commit();
            _logger.LogInformation("Cleared all cart items for UserID {UserID}", userId);
        }
    }
}
