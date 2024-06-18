using AgriculturalForum.Web.Models;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.EntityFrameworkCore;

namespace AgriculturalForum.Web.Interfaces
{
    public interface IPostRepository
    {
        Task<IEnumerable<Post>> GetPostsByCategory(int? catId);
        Task<IEnumerable<Post>> GetAll();
        Task<Post?> GetById(int id);
        Task<int> Add(Post model, IFormFile? imgFile );
        Task<bool> Update(Post model, IFormFile? imgFile);
        Task Delete(int id);
        Task<bool> DoesPostExist(int id);
        Task<IEnumerable<Post>> GetPostsBySearchValue(string  searchValue);
        Task<IEnumerable<Post>> GetPostsByUserId(int userId);
        Task<IEnumerable<Post>> GetSimilarPosts(int? catId, int postId);
        Task<int> GetTotalPosts();
        Task<IEnumerable<Post>> GetRecentPosts();
    }
}
