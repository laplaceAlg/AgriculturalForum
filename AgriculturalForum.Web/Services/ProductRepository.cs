using AgriculturalForum.Web.Helper;
using AgriculturalForum.Web.Interfaces;
using AgriculturalForum.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace AgriculturalForum.Web.Services
{
    public class ProductRepository : IProductRepository
    {
        private readonly KltnDbContext _dbContext;

        public ProductRepository(KltnDbContext dbContext)
        {
            _dbContext = dbContext;
        }
   
        public async Task<int> Add(Product model, IFormFile imgfile, List<IFormFile> additionalImage)
        {
            if (imgfile != null && imgfile.Length > 0)
            {
                string extension = Path.GetExtension(imgfile.FileName);
                string image = $"product_{DateTime.Now.Ticks}" + extension;
                string fileName = ApplicationContext.UploadFile(imgfile, @"uploads/productImages", image);
                model.Image = fileName;
            }
            var newProduct = new Product
            {
                Name = model.Name,
                CreateDate = DateTime.Now,
                Price = model.Price,
                Description = model.Description,
                Image = model.Image,
                IsSelling = true,
                UserId = model.UserId,
                CategoryProductId = model.CategoryProductId,
            };
            _dbContext.Products.Add(newProduct);
            await _dbContext.SaveChangesAsync();
            if (additionalImage != null && additionalImage.Count > 0)
            {
                foreach (var file in additionalImage)
                {
                    string extension1 = Path.GetExtension(file.FileName);
                    string image1 = $"product_{DateTime.Now.Ticks}" + extension1;
                    string additionalImagePath = ApplicationContext.UploadFile(file, @"uploads/productImages", image1);

                    if (!additionalImagePath.Equals("-1"))
                    {

                        var productImage = new ProductImage
                        {
                            Image = additionalImagePath,
                            ProductId = newProduct.Id,
                        };
                        newProduct.ProductImages.Add(productImage);
                    }
                }
                await _dbContext.SaveChangesAsync();
            }
            return newProduct.Id;
        }


        public async Task<IEnumerable<Product>> ListOfProducts(int? catId, string searchValue)
        {
            var data = _dbContext.Products.Where(x => x.IsSelling).AsNoTracking().AsQueryable();

            if (catId.HasValue && catId != 0)
            {
                data = data.Where(x => x.CategoryProductId == catId);
            }

            if (!string.IsNullOrEmpty(searchValue))
            {
                data = data.Where(x => x.Name.Contains(searchValue));
            }

            return await data.OrderByDescending(x => x.CreateDate).ToListAsync();
        }

        public async Task<bool> Update(Product model, IFormFile imgfile, List<IFormFile> additionalImage)
        {
            var productUpdate = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == model.Id);
            if (productUpdate != null)
            {
                productUpdate.Name = model.Name;
                productUpdate.Price = model.Price;
                productUpdate.Description = model.Description;
                productUpdate.IsSelling = model.IsSelling;
                productUpdate.CategoryProductId = model.CategoryProductId;

                // Kiểm tra xem người dùng có tải lên hình ảnh mới hay không
                if (imgfile != null && imgfile.Length > 0)
                {
                    // Xóa hình ảnh cũ
                    string oldImagePath = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"uploads/productImages", productUpdate.Image);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                    string extension = Path.GetExtension(imgfile.FileName);
                    string image = $"product_{DateTime.Now.Ticks}" + extension;
                    productUpdate.Image = ApplicationContext.UploadFile(imgfile, @"uploads/productImages", image);

                }
                if (additionalImage != null && additionalImage.Count > 0)
                {
                    // Xóa các ảnh phụ không còn được tải lên
                    var existingProductImages = _dbContext.ProductImages
                                                .Where(pi => pi.ProductId == productUpdate.Id)
                                                     .ToList();

                    foreach (var existingImage in existingProductImages)
                    {

                        string oldImagePath = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"uploads/productImages", existingImage.Image);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }

                        _dbContext.ProductImages.Remove(existingImage);

                    }

                    await _dbContext.SaveChangesAsync();


                    // Thêm các ảnh phụ mới
                    foreach (var file in additionalImage)
                    {
                        string extension1 = Path.GetExtension(file.FileName);
                        string image1 = $"product_{DateTime.Now.Ticks}" + extension1;
                        string additionalImagePath = ApplicationContext.UploadFile(file, @"uploads/productImages", image1);
                        if (!additionalImagePath.Equals("-1"))
                        {
                            var productImage = new ProductImage
                            {
                                Image = additionalImagePath,
                                ProductId = productUpdate.Id
                            };
                            productUpdate.ProductImages.Add(productImage);
                        }
                    }
                }

                _dbContext.Products.Update(productUpdate);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task Delete(int id)
        {
            var product = await _dbContext.Products.SingleOrDefaultAsync(p => p.Id == id);
            if(product != null)
            {
                var imagePath = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"uploads/productImages", product.Image);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                var additionalImages = await _dbContext.ProductImages.Where(ai => ai.ProductId == product.Id).ToListAsync();
                foreach (var additionalImage in additionalImages)
                {
                    var additionalImagePath = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"uploads/productImages", additionalImage.Image);
                    if (System.IO.File.Exists(additionalImagePath))
                    {
                        System.IO.File.Delete(additionalImagePath);
                    }

                    _dbContext.ProductImages.Remove(additionalImage);
                }

                _dbContext.Products.Remove(product);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> DoesPostExist(int id)
        {
            return await _dbContext.Products.AnyAsync(p => p.Id == id);
        }

        public async Task<Product?> GetById(int id)
        {
            return await _dbContext.Products
            .Include(p => p.ProductImages)
            .Include(c => c.CategoryProduct)
            .Include(u => u.User)
            .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetSimilarProducts(int? catId, int productId)
        {
            return await _dbContext.Products
                             .Include(u => u.User)
                             .Where(p => p.CategoryProductId == catId && p.Id != productId && p.IsSelling)
                             .OrderByDescending(p => p.CreateDate)
                             .Take(8)
                             .ToListAsync();
        }

        public async Task<User?> GetUserById(int id)
        {
            return await _dbContext.Users.SingleOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<Product>> GetProductsByUserId(int userId)
        {
            return await _dbContext.Products.Where(p => p.UserId == userId)
                                            .Include(p => p.User)
                                            .Include(p => p.CategoryProduct)
                                            .OrderByDescending(p => p.CreateDate)
                                            .ThenByDescending(p => p.IsSelling)
                                            .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
           return await _dbContext.Products.AsNoTracking()
                .Where(x => x.IsSelling)
                .Include(u => u.User)
                .OrderByDescending(x => x.CreateDate)
                .ToListAsync();
        }
    }
}
