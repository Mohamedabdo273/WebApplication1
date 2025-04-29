using Stripe.Climate;
using WebApplication1.Models;
using WebApplication1.Repository.IRepository;
using WebApplication1.Services.Iservices;

namespace WebApplication1.Services
{
    public class ProductService : IProductService
    {
        private readonly string _imageFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "ProductImages");
        private readonly IProduct _product;

        public ProductService(IProduct product)
        {
            this._product = product;
        }

        public async Task<Models.Product> CreateProductAsync(Models.Product product, IFormFile? productImg)
        {
            product.ImgUrl = await SaveImageAsync(productImg);
            _product.Create(product);
             _product.Commit();  // Assuming Commit() has async version, otherwise use Task.CompletedTask
            return product;
        }

        public async Task<Models.Product?> UpdateProductAsync(int productId, Models.Product product, IFormFile? productImg)
        {
            var existingProduct = _product.GetOne(expression: p => p.Id == productId);
            if (existingProduct == null) return null;

            if (productImg != null && productImg.Length > 0)
            {
                await DeleteImage(existingProduct.ImgUrl);
                existingProduct.ImgUrl = await SaveImageAsync(productImg);
            }

            existingProduct.Name = product.Name;
            existingProduct.Discription = product.Discription;
            existingProduct.Price = product.Price;
            existingProduct.CategoryId = product.CategoryId;

            _product.Edit(existingProduct);
            _product.Commit();  // Use await if Commit is async

            return existingProduct;  // Return updated product
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            var product = _product.GetOne(expression: p => p.Id == productId);
            if (product == null) return false;

            if (!string.IsNullOrEmpty(product.ImgUrl))
            {
                await DeleteImage(product.ImgUrl);
            }

            _product.Delete(product);
            _product.Commit();  // Use await if Commit is async
            return true;
        }

        private async Task<string> SaveImageAsync(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0) return string.Empty;

            EnsureImageDirectoryExists();
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            var filePath = Path.Combine(_imageFolderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return fileName;
        }

        private async Task DeleteImage(string? fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;

            var filePath = Path.Combine(_imageFolderPath, fileName);
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));  // Asynchronous file delete
            }
        }

        private void EnsureImageDirectoryExists()
        {
            if (!Directory.Exists(_imageFolderPath))
            {
                Directory.CreateDirectory(_imageFolderPath);
            }
        }

        public Models.Product GetProductById(int id)
        {
            return _product.GetOne(expression: e => e.Id == id);
        }
        public IEnumerable<Models.Product> GetAllProduct()
        {
            return _product.Get();
        }

       
       
    }
}
