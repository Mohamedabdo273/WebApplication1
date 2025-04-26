using WebApplication1.Models;

namespace WebApplication1.Services.Iservices
{
    public interface IProductService
    {
        Task<Product> CreateProductAsync(Product product, IFormFile? productImg);
        Product GetProductById(int id);
        Task<Product?> UpdateProductAsync(int productId, Product product, IFormFile? productImg);  // Accept ID and updated product
        Task<bool> DeleteProductAsync(int productId);
        IEnumerable<Models.Product> GetAllOrdersProduct();
    }
}
