using AgriculturalForum.Web.Helper;
using AgriculturalForum.Web.Interfaces;
using AgriculturalForum.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices.JavaScript;

namespace AgriculturalForum.Web.Services
{
    public class PostRepository : IPostRepository
    {
        private readonly KltnDbContext _dbContext;
        public static int PAGE_SIZE { get; set; } = 3;
        public PostRepository(KltnDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<int> Add(Post model, IFormFile? imgFile = null)
        {
            if (imgFile != null && imgFile.Length > 0)
            {
                string extension = Path.GetExtension(imgFile.FileName);
                string image = $"post_{DateTime.Now:yyyyMMdd_HHmmss}{extension}";
                string fileName = ApplicationContext.UploadFile(imgFile, @"uploads/postImages", image);
                model.Image = fileName;
            }

            var post = new Post
            {
                Title = model.Title,
                Content = model.Content,
                Image = model.Image,
                CategoryPostId = model.CategoryPostId,
                UserId = model.UserId,
                CreateDate = DateTime.Now
            };

            _dbContext.Posts.Add(post);
            await _dbContext.SaveChangesAsync();
            return post.Id;
        }

        public async Task<bool> Update(Post model, IFormFile? imgFile = null)
        {
            var post = await _dbContext.Posts.SingleOrDefaultAsync(p => p.Id == model.Id);
            if (post != null)
            {
                post.Title = model.Title;
                post.Content = model.Content;
                post.CategoryPostId = model.CategoryPostId;

                if (imgFile != null && imgFile.Length > 0)
                {
                    if (post.Image != "default.jpg")
                    {
                        string oldImagePath = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"uploads/postImages", post.Image);
                        if (File.Exists(oldImagePath))
                        {
                            File.Delete(oldImagePath);
                        }
                    }

                    string extension = Path.GetExtension(imgFile.FileName);
                    string image = $"post_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}" + extension;
                    string fileName = ApplicationContext.UploadFile(imgFile, @"uploads/postImages", image);
                    post.Image = fileName;
                }

                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task Delete(int id)
        {
            var post = await _dbContext.Posts.SingleOrDefaultAsync(p => p.Id == id);
            if (post != null)
            {
                if (post.Image != "default.jpg")
                {
                    var imagePath = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"uploads/postImages", post.Image);
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }
                }
                var repliesOfPost = await _dbContext.PostReplies.Where(p => p.PostId == post.Id).ToListAsync();
                foreach (var item in repliesOfPost)
                {
                    _dbContext.PostReplies.Remove(item);
                }
                _dbContext.Posts.Remove(post);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> DoesPostExist(int id)
        {
            return await _dbContext.Posts.AnyAsync(item => item.Id == id);
        }

        public async Task<IEnumerable<Post>> GetAll()
        {
            return await _dbContext.Posts
                   .OrderByDescending(x => x.CreateDate)
                   .Include(p => p.User)
                   .Include(p => p.PostReplies)
                   .Include(p => p.CategoryPost)
                   .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetPostsByCategory(int? catId)
        {
            var lsPosts = _dbContext.Posts.AsQueryable();

            if (catId.HasValue)
                lsPosts = lsPosts.Where(p => p.CategoryPostId == catId);

            return await lsPosts
                .Include(p => p.User)
                .Include(p => p.PostReplies)
                .Include(p => p.CategoryPost)
                .OrderByDescending(p => p.CreateDate)
                .ToListAsync();
        }

        public async Task<Post?> GetById(int id)
        {
            return await _dbContext.Posts
                .Include(p => p.User)
                .Include(p => p.PostReplies)
                .ThenInclude(q => q.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Post>> GetSimilarPosts(int? catId, int postId)
        {
            return await _dbContext.Posts
                .Where(p => p.CategoryPostId == catId && p.Id != postId)
                .OrderByDescending(p => p.CreateDate)
                .Take(5)
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetPostsBySearchValue(string searchValue)
        {
            var lsPosts = _dbContext.Posts.AsQueryable();

            if (!string.IsNullOrEmpty(searchValue))
                lsPosts = _dbContext.Posts
                           .AsNoTracking()
                           .Where(x => x.Title.Contains(searchValue) || x.Content.Contains(searchValue));

            return await lsPosts
                .Include(p => p.User)
                .Include(p => p.PostReplies)
                .Include(p => p.CategoryPost)
                .OrderByDescending(p => p.CreateDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetPostsByUserId(int userId)
        {
            return await _dbContext.Posts
                .AsNoTracking()
                .Where(p => p.UserId == userId)
                .Include(p => p.PostReplies)
                .Include(x => x.CategoryPost)
                .Include(p => p.User)
                .OrderByDescending(p => p.CreateDate).ToListAsync();
        }

        public async Task<int> GetTotalPosts()
        {
            return await _dbContext.Posts.CountAsync();
        }

        public async Task<IEnumerable<Post>> GetRecentPosts()
        {
            return await _dbContext.Posts
                            .OrderByDescending(p => p.CreateDate)
                            .Take(5)
                            .ToListAsync();
        }
    }
}

