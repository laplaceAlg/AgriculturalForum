using AgriculturalForum.Web.Models;
using System.Collections;
using System.Threading.Tasks;

namespace AgriculturalForum.Web.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> ListOfProducts(int? catId, string searchValue);
        Task<IEnumerable<Product>> GetAll();
        Task<Product?> GetById(int id);
        Task<IEnumerable<Product>> GetSimilarProducts(int? catId, int productId);
        Task<int> Add(Product model, IFormFile imgfile, List<IFormFile> additionalImage);
        Task<bool> Update(Product model, IFormFile imgfile, List<IFormFile> additionalImage);
        Task Delete(int id);
        Task<bool> DoesPostExist(int id);
        Task<User?> GetUserById(int id);
        Task<IEnumerable<Product>> GetProductsByUserId (int userId);
    }
}
