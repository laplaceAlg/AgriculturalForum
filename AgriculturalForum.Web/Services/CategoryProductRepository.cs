using AgriculturalForum.Web.Interfaces;
using AgriculturalForum.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace AgriculturalForum.Web.Services
{
    public class CategoryProductRepository : ICategoryProductRepository
    {
        private readonly KltnDbContext _dbContext;

        public CategoryProductRepository(KltnDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<int> Add(CategoryProduct model)
        {
            var existingCategory = await _dbContext.CategoryProducts.FirstOrDefaultAsync(c => c.Name == model.Name);
            if (existingCategory != null)
                return -1;
            var category = new CategoryProduct
            {
                Name = model.Name,
                Description = model.Description,
                CreateDate = DateTime.Now,
                IsActive = model.IsActive
            };
            _dbContext.CategoryProducts.Add(category);
            await _dbContext.SaveChangesAsync();
            return category.Id;
        }

        public async Task<bool> Update(CategoryProduct model)
        {
            var existingCategory = await _dbContext.CategoryProducts.FirstOrDefaultAsync(c => c.Name == model.Name && c.Id != model.Id);
            if (existingCategory != null)
                return false;
            var data = await _dbContext.CategoryProducts.SingleOrDefaultAsync(p => p.Id == model.Id);
            if (data != null)
            {
                data.Name = model.Name;
                data.Description = model.Description;
                data.IsActive = model.IsActive;
                _dbContext.CategoryProducts.Update(data);
                await _dbContext.SaveChangesAsync();
            }
            return true;
        }

        public async Task Delete(int id)
        {
            if (!await IsUsed(id))
            {
                var data = await _dbContext.CategoryProducts.SingleOrDefaultAsync(p => p.Id == id);
                if (data != null)
                {
                    _dbContext.CategoryProducts.Remove(data);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task<bool> DoesCatExist(int id)
        {
            return await _dbContext.CategoryProducts.AnyAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<CategoryProduct>> GetAll()
        {
            var data = await _dbContext.CategoryProducts.ToListAsync();
            return data;
        }

        public async Task<CategoryProduct?> GetById(int id)
        {
            var data = await _dbContext.CategoryProducts.SingleOrDefaultAsync(p => p.Id == id);
            if (data != null)
            {
                return data;
            }
            return null;
        }

        public async Task<bool> IsUsed(int id)
        {
            return await _dbContext.Products.AnyAsync(c => c.CategoryProductId == id);
        }

        public async Task<List<CategoryProduct>> GetCategoryProducts(bool? isActive, string searchValue)
        {
            var data = _dbContext.CategoryProducts.AsNoTracking().AsQueryable();

            if (isActive.HasValue)
            {
                data = data.Where(x => x.IsActive == isActive);
            }

            if (!string.IsNullOrEmpty(searchValue))
            {
                data = data.Where(x => x.Name.Contains(searchValue));
            }

            return await data.OrderByDescending(x => x.CreateDate).ToListAsync();
        }

        public async Task<int> GetTotalCategoryProducts()
        {
           return await _dbContext.CategoryProducts.CountAsync();
        }
    }
}
