using AgriculturalForum.Web.Models;

namespace AgriculturalForum.Web.Interfaces
{
    public interface IReplyRepository
    {
        Task<IEnumerable<PostReply>> GetAll();
        Task<PostReply?> GetById(int id);
        Task<int> Add(PostReply model);
        Task<bool> Update(PostReply model);
        Task Delete(int id);
        Task<bool> DoesPostExist(int id);
        Task<int> GetTotalReplies();
    }
}
