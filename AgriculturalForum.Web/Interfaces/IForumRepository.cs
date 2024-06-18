using AgriculturalForum.Web.Models;

namespace AgriculturalForum.Web.Interfaces
{
    public interface IForumRepository
    {
        Task<IEnumerable<CategoryPost>> GetCatOfPosts();
        Task<IEnumerable<Post>> GetLatestPostOfCat();

    }
}
