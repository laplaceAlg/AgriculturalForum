using AgriculturalForum.Web.Interfaces;
using AgriculturalForum.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AgriculturalForum.Web.Services
{
    public class CategoryPostRepository : ICategoryPostRepository
    {
        private readonly KltnDbContext _dbContext;

        public CategoryPostRepository(KltnDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<int> Add(CategoryPost model)
        {
            var existingCategory = await _dbContext.CategoryPosts.FirstOrDefaultAsync(c => c.Title == model.Title);
            if (existingCategory != null)
                return -1;
            var category = new CategoryPost
            {
                Title = model.Title,
                Description = model.Description,
                CreateDate = DateTime.Now,
                IsActive = model.IsActive
            };
            _dbContext.CategoryPosts.Add(category);
            await _dbContext.SaveChangesAsync();
            return category.Id;
        }

        public async Task Delete(int id)
        {
            if (!await IsUsed(id))
            {
                var data =  await _dbContext.CategoryPosts.SingleOrDefaultAsync(p => p.Id == id);
                if (data != null)
                {
                    _dbContext.CategoryPosts.Remove(data);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task<bool> Update(CategoryPost model)
        {
            var existingCategory = await _dbContext.CategoryPosts.FirstOrDefaultAsync(c => c.Title == model.Title && c.Id != model.Id);
            if (existingCategory != null)
                return false;
            var data = await _dbContext.CategoryPosts.SingleOrDefaultAsync(p => p.Id == model.Id);
            if (data != null)
            {
                data.Title = model.Title;
                data.Description = model.Description;
                data.IsActive = model.IsActive;
                _dbContext.CategoryPosts.Update(data);
                await _dbContext.SaveChangesAsync();
            }
            return true;
        }

        public async Task<bool> DoesCatExist(int id)
        {
            return await _dbContext.CategoryPosts.AnyAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<CategoryPost>> GetAll()
        {
            var data =  await _dbContext.CategoryPosts.ToListAsync();
            return data;
        }

        public async Task<CategoryPost?> GetById(int id)
        {
            var data = await _dbContext.CategoryPosts.SingleOrDefaultAsync(p => p.Id == id);
            if (data != null)
            {
                return data;
            }
            return null;
        }

        public async Task<bool> IsUsed(int id)
        {
            return await _dbContext.Posts.AnyAsync(c => c.CategoryPostId == id);
        }
    
        public async Task<List<CategoryPost>> GetCategoryPosts(bool? isActive, string searchValue)
        {
            var data = _dbContext.CategoryPosts.AsNoTracking().AsQueryable();

            if (isActive.HasValue)
            {
                data = data.Where(x => x.IsActive == isActive);
            }

            if (!string.IsNullOrEmpty(searchValue))
            {
                data = data.Where(x => x.Title.Contains(searchValue));
            }

            return await data.OrderByDescending(x => x.CreateDate).ToListAsync();
        }

        public async Task<int> GetTotalCategoryPosts()
        {
            return await _dbContext.CategoryPosts.CountAsync();
        }
    }
}
