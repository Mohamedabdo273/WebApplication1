using WebApplication1.Models;

namespace WebApplication1.Services.Iservices
{
    public interface ICartService
    {
        IEnumerable<Cart> GetUserCart(string userId);
        Cart GetCartItem(int cartId);
        void AddToCart(Cart cartItem);
        void UpdateCartItem(int id, Cart updatedItem);
        bool RemoveFromCart(int id);
        void ClearCart(string userId);
}
}
