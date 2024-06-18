using AgriculturalForum.Web.Models;


namespace AgriculturalForum.Web.Interfaces
{
    public interface ICategoryProductRepository
    {
        Task<IEnumerable<CategoryProduct>> GetAll();
        Task<CategoryProduct?> GetById(int id);
        Task<int> Add(CategoryProduct model);
        Task<bool> Update(CategoryProduct model);
        Task Delete(int id);
        Task<bool> DoesCatExist(int id);
        Task<bool> IsUsed(int id);
        Task<List<CategoryProduct>> GetCategoryProducts(bool? isActive, string searchValue);
        Task<int> GetTotalCategoryProducts();
    }
}
