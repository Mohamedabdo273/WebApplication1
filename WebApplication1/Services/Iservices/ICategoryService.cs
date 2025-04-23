using WebApplication1.Models;

namespace WebApplication1.Services.Iservices
{
    public interface ICategoryService
    {
        IEnumerable<Category> GetAllCategories();

        Category GetCategoryById(int id);

        void AddCategory(Category category);

        void UpdateCategory(int id, Category category);

        void DeleteCategory(int id);
    }
}
