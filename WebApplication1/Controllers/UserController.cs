using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Services.Iservices;
using WebApplication1.Utility;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles =SD.CustomerRole)]
    public class UserController : ControllerBase
    {

        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrderService orderService;

        public UserController(IProductService productService, ICartService cartService, UserManager<ApplicationUser> userManager, IOrderService orderService)
        {
            _productService = productService;
            _cartService = cartService;
            _userManager = userManager;
            this.orderService = orderService;
        }

        // Get all products with optional filters
        [HttpGet("Products")]
        [AllowAnonymous]
        public IActionResult GetProducts(string? search = null, int page = 1, string? category = null, decimal? minPrice = null, decimal? maxPrice = null)
        {

            int pageSize = 5;

            var products = _productService.GetAllProduct();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
                products = products.Where(e => e.Name.Contains(search));
            }

            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(e => e.Category.Name == category);
            }

            if (minPrice.HasValue)
            {
                products = products.Where(e => e.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                products = products.Where(e => e.Price <= maxPrice.Value);
            }

            int totalProducts = products.Count();
            int totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            if (page > totalPages) page = totalPages;

            var paginatedProducts = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new
            {
                Data = paginatedProducts,
                TotalPages = totalPages,
                CurrentPage = page
            });

        }

        // Get product details by ID
        [HttpGet("Product/{id}")]
        [AllowAnonymous]
        public IActionResult GetProductDetails(int id)
        {
            var product = _productService.GetProductById(id);
            var Detais = new
            {
                ProductName = product.Name,
                Discription = product.Discription,
                Price = product.Price,
                Discount = product.Discount,
                Img = product.ImgUrl,
                CategoryName = product.Category.Name,
                Count=product.Count,
                Brand = product.Brand,
                Model = product.Model
            };
            return Ok(Detais);
        }

        // Add item to user's cart
        [HttpPost("AddToCart")]
        [Authorize]
        public IActionResult AddToCart(int productId, int count)
        {
            var appUser = _userManager.GetUserId(User);
            if (appUser == null)
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }

            var productItem = _productService.GetProductById(productId);
            if (productItem == null)
            {
                return NotFound(new { message = "Product not found." });
            }

            var existingCart = _cartService.GetUserCart(appUser)
                                          .FirstOrDefault(e => e.ProductId == productId);

            if (existingCart == null)
            {
                _cartService.AddToCart(new Cart
                {
                    Count = count,
                    ProductId = productId,
                    UserID = appUser
                });
            }
            else
            {
                existingCart.Count += count;
                _cartService.UpdateCartItem(existingCart.Id, existingCart);
            }
            return Ok(new { message = "Product added to cart." });
        }



        // Get user's cart items
        [HttpGet("Cart/GetItems")]
        [Authorize]
        public IActionResult GetCartItems()
        {

            var userId = _userManager.GetUserId(User);
            var cartItems = _cartService.GetUserCart(userId).ToList();
            var item = cartItems.Select(item => new
            {
                Id=item.Id,
                count=item.Count,
                ProductId=item.ProductId,
                ProductName=item.Product.Name,
                ProductImg=item.Product.ImgUrl
            }).ToList();
            return Ok(item);

        }

        // Increment cart item count
        [HttpPut("Cart/Increment/{id}")]
        [Authorize]
        public IActionResult IncrementCartItem(int id)
        {
            var userId = _userManager.GetUserId(User);

            var cartItem = _cartService.GetCartItem(id);

            var product = _productService.GetProductById(cartItem.ProductId);

            if (cartItem.Count + 1 > product.Count)
            {
                return BadRequest(new { message = "Cannot exceed available stock." });
            }

            cartItem.Count++;
            _cartService.UpdateCartItem(id, cartItem);

            return Ok(new { message = "Cart item incremented." });

        }

        // Decrement cart item count
        [HttpPut("Cart/Decrement/{id}")]
        [Authorize]
        public IActionResult DecrementCartItem(int id)
        {
            var cartItem = _cartService.GetCartItem(id);
            if (cartItem == null)
            {
                return NotFound(new { message = "Cart item not found." });
            }

            cartItem.Count--;
            if (cartItem.Count <= 0)
            {
                _cartService.RemoveFromCart(id);
                return Ok(new { message = "Cart item removed." });
            }

            _cartService.UpdateCartItem(id, cartItem);
            return Ok(new { message = "Cart item decremented." });
        }

        // Delete item from the cart
        [HttpDelete("Cart/Delete/{id}")]
        [Authorize]
        public IActionResult DeleteCartItem(int id)
        {

            var cartItem = _cartService.GetCartItem(id);

            _cartService.RemoveFromCart(id);

            return Ok(new { message = "Cart item deleted." });

        }

        //----------------------------------------Order------------------------------------------ -

        [HttpGet("UserOrders")]
        [Authorize]
        public async Task<IActionResult> UserOrders()
        {
            var appUserId = _userManager.GetUserId(User);

            var userOrders = await orderService.GetAllOrdersAsync();
            var userOrderList = userOrders.Where(order => order.UserID == appUserId).ToList();


            var orderDetails = userOrderList.Select(order => new
            {
                Id = order.Id,
                ProductId = order.ProductID,
                ProductName = order.Product?.Name,
                Count = order.Count,
                Date = order.Date
            }).ToList();

            return Ok(orderDetails);

        }
    }

}
