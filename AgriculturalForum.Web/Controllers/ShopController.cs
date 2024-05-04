using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PagedList.Core;
using System.Buffers;
using System.Drawing.Printing;
using System.Runtime.InteropServices.JavaScript;
using AgriculturalForum.Web.Helper;
using AgriculturalForum.Web.Models;
using AgriculturalForum.Web.Extensions;

namespace AgriculturalForum.Web.Controllers
{
    public class ShopController : Controller
    {
        private readonly KltnDbContext _dbContext;
        private LanguageService _localization;
        const string CREATE_TITLE = "NewProduct";
        const string EDIT_TITLE = "UpdateProduct";
        public ShopController(KltnDbContext dbContext, LanguageService localization)
        {
            _dbContext = dbContext;
            _localization = localization;
        }

        public IActionResult Index(int page = 1, int id = 0, string searchValue = "")
        {
            var pageNumber = page;
            var pageSize = 8;

            List<Product> lsProducts = new List<Product>();
            lsProducts = _dbContext.Products.Where(x => x.IsSelling == true)
             .AsNoTracking()
             .Include(x => x.CategoryProduct)
             .Include(x => x.ProductImages)
             .Include(x => x.User)
             .OrderByDescending(x => x.CreateDate).ToList();
            if (id != 0)
            {
                lsProducts = _dbContext.Products.Where(x => x.IsSelling == true)
                .AsNoTracking()
                .Where(x => x.CategoryProductId == id)
                .Include(x => x.CategoryProduct)
                .Include(X => X.ProductImages)
                .Include(x => x.User)
                .OrderByDescending(x => x.CreateDate).ToList();
            }


            if (!string.IsNullOrWhiteSpace(searchValue))
            {

                lsProducts = _dbContext.Products
                .AsNoTracking()
                .Where(x => x.Name.Contains(searchValue) && x.IsSelling == true)
                .Include(x => x.CategoryProduct)
                .Include(X => X.ProductImages)
                .Include(x => x.User)
                .OrderByDescending(x => x.CreateDate).ToList();
            }

            PagedList<Product> models = new PagedList<Product>(lsProducts.AsQueryable(), pageNumber, pageSize);
            ViewBag.ListOfCategories = _dbContext.CategoryProducts.ToList();
            ViewBag.CurrentCateID = id;
            ViewBag.CateName = _dbContext.CategoryProducts.Where(x => x.Id == id).Select(x => x.Name).FirstOrDefault();
            ViewBag.CurrentPage = pageNumber;
            ViewBag.SearchValue = searchValue;

            return View(models);
        }

        public IActionResult Detail(int id)
        {
            var product = _dbContext.Products
                .Include(p => p.ProductImages)
                .Include(c => c.CategoryProduct)
                .Include(u => u.User)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var similarProducts = _dbContext.Products
                              .Include(u => u.User)
                              .Where(p => p.CategoryProductId == product.CategoryProductId && p.Id != id)
                              .OrderByDescending(p => p.CreateDate)
                              .Take(8)
                              .ToList();

            ViewBag.similarProducts = similarProducts;
            return View(product);
        }

        public IActionResult Create()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account", new { ReturnUrl = "/Shop/Create" });
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == int.Parse(userId));
            if (user != null && (string.IsNullOrEmpty(user.Address) || string.IsNullOrEmpty(user.Province)))
                return RedirectToAction("Detail", "Account");
            ViewBag.Title = _localization.Getkey(CREATE_TITLE);
            ViewBag.Categories = _dbContext.CategoryProducts.ToList();
            var model = new Product()
            {
                Id = 0,
                IsSelling = true
            };
            return View("Edit", model);
        }

        public IActionResult Edit(int id = 0)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account", new { ReturnUrl = "/Shop/Edit" });
            ViewBag.Title = _localization.Getkey(EDIT_TITLE);
            ViewBag.Categories = _dbContext.CategoryProducts.ToList();
            var model = _dbContext.Products.FirstOrDefault(p => p.Id == id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }

        [HttpPost] //Attribute => chỉ nhân dữ liệu gửi lên dưới dạng Post
        [ValidateAntiForgeryToken]
        public IActionResult Save(Product product, IFormFile imgfile, List<IFormFile> additionalImage)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");
            var account = _dbContext.Users.AsNoTracking().FirstOrDefault(u => u.Id == int.Parse(userId));
            if (account == null)
                return NotFound();


            //TODO: Kiểm soát dữ liệu trong model xem có hợp lệ hay không?

            if (string.IsNullOrWhiteSpace(product.Name))
                ModelState.AddModelError(nameof(product.Name), _localization.Getkey("ProductNameRequired"));
            if (product.Price < 0 || product.Price == null)
                ModelState.AddModelError(nameof(product.Price), _localization.Getkey("PriceValidation"));
            if (product.CategoryProductId == null)
                ModelState.AddModelError(nameof(product.CategoryProductId), _localization.Getkey("CategoryRequired"));

            if (!ModelState.IsValid)
            {
                ViewBag.Title = product.Id == 0 ? _localization.Getkey(CREATE_TITLE) : _localization.Getkey(EDIT_TITLE);
                ViewBag.Categories = _dbContext.CategoryProducts.ToList();
                return View("Edit", product);
            }

            if (product.Id == 0)
            {
                if (imgfile == null || imgfile.Length == 0)
                {
                    ModelState.AddModelError("imgfile", _localization.Getkey("ImageRequired"));
                    ViewBag.Title = CREATE_TITLE;
                    ViewBag.Categories = _dbContext.CategoryProducts.ToList();
                    return View("Edit", product);
                }
                else
                {
                    string extension = Path.GetExtension(imgfile.FileName);
                    string image = $"product_{DateTime.Now.Ticks}" + extension;
                    string fileName = ApplicationContext.UploadFile(imgfile, @"uploads/productImages", image);
                    var newProduct = new Product
                    {
                        Id = product.Id,
                        Name = product.Name,
                        CreateDate = DateTime.Now,
                        Price = product.Price,
                        Description = product.Description,
                        Image = fileName,
                        IsSelling = true,
                        UserId = account.Id,
                        CategoryProductId = product.CategoryProductId,
                    };
                    _dbContext.Products.Add(newProduct);
                    _dbContext.SaveChanges();
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
                        _dbContext.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
            }
            else
            {
                var productUpdate = _dbContext.Products.FirstOrDefault(p => p.Id == product.Id);
                if (productUpdate != null)
                {
                    productUpdate.Name = product.Name;
                    productUpdate.Price = product.Price;
                    productUpdate.Description = product.Description;
                    productUpdate.IsSelling = product.IsSelling;
                    productUpdate.CategoryProductId = product.CategoryProductId;

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

                        _dbContext.SaveChanges();


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

                    _dbContext.Update(productUpdate);
                    _dbContext.SaveChanges();
                    return RedirectToAction("Index", "User", new { id = productUpdate.UserId });
                }
            }
            ViewBag.Categories = _dbContext.CategoryProducts.ToList();
            return View("Edit", product);
        }


        public IActionResult Delete(int id = 0)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");
            var product = _dbContext.Products.Find(id);
            if (Request.Method == "POST")
            {

                var imagePath = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"uploads/productImages", product.Image);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                var additionalImages = _dbContext.ProductImages.Where(ai => ai.ProductId == product.Id).ToList();
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
                _dbContext.SaveChanges();
                return RedirectToAction("MyPost", new { id = product.UserId });
            }
            else
            {
                return View(product);
            }
        }

        public IActionResult MyPost(int id, int page = 1)
        {
            var pageNumber = page;
            var pageSize = 8;

            var user = _dbContext.Users.Where(p => p.Id == id).FirstOrDefault();
            ViewBag.User = user;
            List<Product> lsProducts = new List<Product>();
            lsProducts = _dbContext.Products.Where(p => p.UserId == id && p.IsSelling == true)
                                            .OrderByDescending(p => p.CreateDate)
                                            .ToList();
            PagedList<Product> models = new PagedList<Product>(lsProducts.AsQueryable(), pageNumber, pageSize);
            return View(models);
        }
    }
}
