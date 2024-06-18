using AgriculturalForum.Web.Interfaces;
using AgriculturalForum.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace AgriculturalForum.Web.Services
{
    public class ReplyRepository : IReplyRepository
    {
        private readonly KltnDbContext _dbContext;

        public ReplyRepository(KltnDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> Add(PostReply model)
        {
            var reply = new PostReply
            {
                Content = model.Content,
                CreateDate = DateTime.Now,
                PostId = model.PostId,
                UserId = model.UserId,
            };
            _dbContext.PostReplies.Add(reply);
            await _dbContext.SaveChangesAsync();
            return reply.Id;
        }

        public async Task<bool> Update(PostReply model)
        {
            var replyUpdate = await _dbContext.PostReplies.SingleOrDefaultAsync(p => p.Id == model.Id);
           if(replyUpdate != null)
            {
                replyUpdate.Content = model.Content;
                _dbContext.PostReplies.Update(replyUpdate);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task Delete(int id)
        {
            var reply = _dbContext.PostReplies.SingleOrDefault(p => p.Id == id);
            if (reply != null)
            {
                _dbContext.PostReplies.Remove(reply);
               await _dbContext.SaveChangesAsync();
            }
        }

        public Task<bool> DoesPostExist(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PostReply>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<PostReply?> GetById(int id)
        {
            return await _dbContext.PostReplies.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<int> GetTotalReplies()
        {
           return await _dbContext.PostReplies.CountAsync();
        }
    }
}
