using AgriculturalForum.Web.Interfaces;
using AgriculturalForum.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace AgriculturalForum.Web.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly KltnDbContext _dbContext;
        public UserRepository(KltnDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> GetLatestMember()
        {
            return await _dbContext.Users.OrderByDescending(u => u.MemberSince).FirstOrDefaultAsync();
        }

        public async Task<int> GetTotalMembers()
        {
            return await _dbContext.Users.CountAsync();
        }

        public async Task<int> GetTotalPosts(int userId)
        {
            return await _dbContext.Posts.Where(p => p.UserId == userId).CountAsync();
        }

        public async Task<int> GetTotalProducts(int userId)
        {
            return await _dbContext.Products.Where(p => p.UserId == userId).CountAsync();
        }

        public async Task<User?> GetUserById(int id)
        {
            return await _dbContext.Users
                .Include(u => u.Posts)
                .Include(u => u.PostReplies)
                .Include(u => u.Products)
                .SingleOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<User>> ListOfUsers(bool? isActive, string searchValue)
        {
            var data = _dbContext.Users.AsNoTracking().AsQueryable();

            if (isActive.HasValue)
            {
                data = data.Where(x => x.IsActive == isActive);
            }

            if (!string.IsNullOrEmpty(searchValue))
            {
                data = data.Where(x => x.FullName.Contains(searchValue) || x.Email.Contains(searchValue));
            }

            return await data.OrderByDescending(x => x.MemberSince).ToListAsync();
        }

        public async Task<bool> Update(User model)
        {
            var userUpdate = _dbContext.Users.FirstOrDefault(u => u.Id == model.Id);
            if (userUpdate != null)
            {
                userUpdate.IsActive = model.IsActive;
                _dbContext.Users.Update(userUpdate);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
