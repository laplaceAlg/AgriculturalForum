using AgriculturalForum.Web.Interfaces;
using AgriculturalForum.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace AgriculturalForum.Web.Services
{
    public class ForumRepository : IForumRepository
    {
        private readonly KltnDbContext _dbContext;

        public ForumRepository(KltnDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<CategoryPost>> GetCatOfPosts()
        {
           return await _dbContext.CategoryPosts
                .Where(c => c.IsActive)
                .Include(p => p.Posts)
                .ThenInclude(pr => pr.PostReplies)
                .ToListAsync();
        }

  
        public async Task<IEnumerable<Post>> GetLatestPostOfCat()
        {
           return await _dbContext.Posts
                    .Include(p => p.User)
                    .GroupBy(p => p.CategoryPostId)
                    .Select(g => g.OrderByDescending(p => p.CreateDate).FirstOrDefault())
                    .ToListAsync();
        }
    }
}
