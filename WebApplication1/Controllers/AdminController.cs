using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services.Iservices;
using WebApplication1.Utility;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles =SD.adminRole)]
    public class AdminController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IOrderService orderService;
        private readonly ICategoryService categoryService;

        public AdminController(ICategoryService categoryService, IProductService productService,IOrderService orderService)
        {
            this.categoryService = categoryService;
            _productService = productService;
            this.orderService = orderService;
        }
//------------------------------Category------------------------------------
         [HttpGet("GetAll")]
        [AllowAnonymous] 
        public IActionResult GetAll()
        {
            var categories = categoryService.GetAllCategories();
            return Ok(categories);
        }

        [HttpPost("Create")]
        public IActionResult Create([FromBody] Category category)
        { 
            categoryService.AddCategory(category);
            return Ok(category);

        }

        [HttpPut("Edit/{categoryId}")]
        public IActionResult Edit(int categoryId, [FromBody] Category category)
        {
            
            categoryService.UpdateCategory(categoryId, category);
            return Ok("Category updated successfully.");
        }

        [HttpDelete("Delete/{categoryId}")]
        public IActionResult Delete(int categoryId)
        { 
            categoryService.DeleteCategory(categoryId);
            return Ok($"Category with ID {categoryId} deleted successfully.");
        }
      //  ------------------------------------------Product---------------------------------------
[HttpGet("AllProducts")]
        public IActionResult AllProducts(int pageNumber = 1, string? search = null)
        {
            int pageSize = 10;
            var allProducts = _productService.GetAllProduct();
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                allProducts = allProducts.Where(p =>
                    (!string.IsNullOrEmpty(p.Name) && p.Name.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(p.Brand) && p.Brand.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(p.Model) && p.Model.ToLower().Contains(search))
                ).ToList();
            }

            var paginated = allProducts
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new
            {
                TotalProducts = allProducts.Count(),
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling((double)allProducts.Count() / pageSize),
                Products = paginated
            };

            return Ok(response);
        }

        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromForm] Product product, IFormFile? productImg)
        {
            

            var created = await _productService.CreateProductAsync(product, productImg);
            return Ok(created);
        }

        [HttpGet("GetById/{id}")]
        public IActionResult GetById(int id)
        {
            var product = _productService.GetProductById(id);
            return Ok(product);
        }

        [HttpPut("EditProduct/{id}")]
        public async Task<IActionResult> EditProduct(int id, [FromForm] Product product, IFormFile? productImg)
        {
            

            var updated = await _productService.UpdateProductAsync(id, product, productImg);
            return Ok(updated);
        }

        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var deleted = await _productService.DeleteProductAsync(id);
            return Ok(new { Message = "Product deleted successfully" });
        }


           //-------------------------------------------Order---------------------------------------------
         [HttpGet("AdminOrders")]
        public async Task<IActionResult> Orders()
        {
          
                var orders = await orderService.GetAllOrdersAsync();

                var orderDetails = orders.Select(item => new
                {
                    Id = item.Id,
                    UserName = item.ApplicationUser?.Email,
                    ProductId = item.ProductID,
                    ProductName = item.Product?.Name,
                    Count = item.Count,
                    Date = item.Date
                }).ToList();
                return Ok(orderDetails);
           
        }
    }
}
