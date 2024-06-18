using AgriculturalForum.Web.Models;

namespace AgriculturalForum.Web.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> ListOfUsers(bool? isActive, string searchValue);
        Task<User?> GetUserById(int id);
        Task<int> GetTotalMembers();
        Task<int> GetTotalPosts(int userId);
        Task<int> GetTotalProducts(int userId);
        Task<User?> GetLatestMember();

        Task<bool> Update(User model);
    }
}
