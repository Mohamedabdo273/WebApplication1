using WebApplication1.Models;
using WebApplication1.Repository.IRepository;
using WebApplication1.Services.Iservices;
using Microsoft.Extensions.Logging;

namespace WebApplication1.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategory _category;

        public CategoryService(ICategory category)
        {
            _category = category;
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

            }
        }

        public void DeleteCategory(int id)
        {
            var category = _category.GetOne(expression: e => e.Id == id);
            if (category != null)
            {
                _category.Delete(category);
                _category.Commit();
            }
        }
    }
}
