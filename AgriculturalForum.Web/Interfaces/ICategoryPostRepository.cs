
using AgriculturalForum.Web.Models;

namespace AgriculturalForum.Web.Interfaces
{
    public interface ICategoryPostRepository
    {
        Task<IEnumerable<CategoryPost>> GetAll();
        Task<CategoryPost?> GetById(int id);
        Task<int> Add(CategoryPost model);
        Task<bool> Update(CategoryPost model);
        Task Delete(int id);
        Task<bool> DoesCatExist(int id);
        Task<bool> IsUsed(int id);
        Task<List<CategoryPost>> GetCategoryPosts(bool? isActive, string searchValue);
        Task<int> GetTotalCategoryPosts();
    }
}
