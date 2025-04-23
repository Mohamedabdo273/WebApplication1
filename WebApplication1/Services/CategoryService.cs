using WebApplication1.Models;
using WebApplication1.Repository.IRepository;
using WebApplication1.Services.Iservices;
using Microsoft.Extensions.Logging;

namespace WebApplication1.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategory _category;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ICategory category, ILogger<CategoryService> logger)
        {
            _category = category;
            _logger = logger;
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return _category.Get();
        }

        public Category GetCategoryById(int id)
        {
            return _category.GetOne(expression: e => e.Id == id);
        }

        public void AddCategory(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            _category.Create(category);
            _category.Commit();
            _logger.LogInformation("Added new category: {Name}", category.Name);
        }

        public void UpdateCategory(int id, Category updatedCategory)
        {
            if (updatedCategory == null)
                throw new ArgumentNullException(nameof(updatedCategory));

            var existing = _category.GetOne(expression: e => e.Id == id);
            if (existing != null)
            {
                existing.Name = updatedCategory.Name;
                _category.Edit(existing);
                _category.Commit();

                _logger.LogInformation("Updated category ID {Id} to Name {Name}", id, updatedCategory.Name);
            }
        }

        public void DeleteCategory(int id)
        {
            var category = _category.GetOne(expression: e => e.Id == id);
            if (category != null)
            {
                _category.Delete(category);
                _category.Commit();
                _logger.LogInformation("Deleted category ID {Id}", id);
            }
        }
    }
}
