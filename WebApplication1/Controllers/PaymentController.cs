using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using WebApplication1.Models;
using WebApplication1.Services.Iservices;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICartService _cartService;
        private readonly IProductService _productService;
        public PaymentController(IOrderService orderService,
UserManager<ApplicationUser> userManager,
    ICartService cartRepository,
    IProductService productRepository)
        {
            _orderService = orderService;
            _userManager = userManager;
            _cartService = cartRepository;
            _productService = productRepository;
        }
        [HttpPost("Pay")]
        [Authorize]
        public async Task<IActionResult> Pay()
        {
            var appUserId = _userManager.GetUserId(User);
            if (appUserId == null)
                return Unauthorized(new { message = "User is not authenticated." });

            var cartItems = _cartService.GetUserCart(appUserId).ToList();
            if (!cartItems.Any())
                return BadRequest(new { message = "No items in the cart to pay for." });

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"http://localhost:4200/success", // Redirect to Angular success page
                CancelUrl = $"http://localhost:4200/cart", // Redirect to cart if canceled
            };

            foreach (var item in cartItems)
            {
                if (item.Product.Price < 0)
                    return BadRequest(new { message = "Invalid product price." });

                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                        },
                        UnitAmount = (long)(item.Product.Price * 100),
                    },
                    Quantity = item.Count,
                });
            }

            var service = new SessionService();
            var session = service.Create(options);
            return Ok(new { url = session.Url });
        }

        [HttpGet("Success")]
        [Authorize]
        public async Task<IActionResult> Success()
        {
            var appUserId = _userManager.GetUserId(User);

            var cartItems = _cartService.GetUserCart(appUserId).ToList();
            foreach (var item in cartItems)
            {
                if (item.Product != null)
                {
                    // Create an order
                    await _orderService.AddOrderAsync(new Models.Order
                    {
                        UserID = appUserId,
                        ProductID = item.ProductId,
                        Count = item.Count,
                        Status=OrderStatus.Processing,
                        Date = DateTime.Now
                    });

                    // Update product stock
                    var productItem = _productService.GetProductById(item.Product.Id);
                    if (productItem != null)
                    {
                        productItem.Count -= item.Count;
                    }

                    _cartService.ClearCart(appUserId);
                }
            }
            return Ok(new { message = "Payment successful." });
        }

        [HttpGet("Cancel")]
        [Authorize]
        public IActionResult Cancel()
        {
            return Ok(new { message = "Payment canceled." });
        }

    }
}
