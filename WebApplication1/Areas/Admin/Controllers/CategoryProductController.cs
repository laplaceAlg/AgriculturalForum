using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System.Drawing.Printing;
using WebApplication1.Helpper;
using WebApplication1.Models;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryProductController : Controller
    {
        private readonly KltnDbContext _dbContext;
        const int PAGE_SIZE = 3;
        const string CREATE_TITLE = "Tạo danh mục sản phẩm mới";
        public CategoryProductController(KltnDbContext dbcontext)
        {
            _dbContext = dbcontext;
        }
      
        public IActionResult Index(int page = 1, bool? isActive = null, string searchValue = "")
        {
            var pageNumber = page;
            var pageSize = PAGE_SIZE;

            List<CategoryProduct> lsCategories = new List<CategoryProduct>();
            lsCategories = _dbContext.CategoryProducts
                   .AsNoTracking()
                   .OrderByDescending(x => x.CreateDate)
                   .ToList();
           

            if (!string.IsNullOrEmpty(searchValue) && isActive.HasValue)
            {
                lsCategories = _dbContext.CategoryProducts
                        .AsNoTracking()
                        .Where(x => x.IsActive == isActive && x.Name.Contains(searchValue))
                        .OrderByDescending(x => x.CreateDate)
                        .ToList();
            }
            else if (!string.IsNullOrEmpty(searchValue))
            {
                lsCategories = _dbContext.CategoryProducts
                      .AsNoTracking()
                      .Where(x => x.Name.Contains(searchValue))
                      .OrderByDescending(x => x.CreateDate)
                      .ToList();
            }
            else if (isActive.HasValue)
            {

                lsCategories = _dbContext.CategoryProducts
                    .AsNoTracking()
                    .Where(x => x.IsActive == isActive)
                    .OrderByDescending(x => x.CreateDate)
                    .ToList();
            }


            PagedList<CategoryProduct> models = new PagedList<CategoryProduct>(lsCategories.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            ViewBag.IsAcTive = isActive;
            ViewBag.SearchValue = searchValue;
            List<SelectListItem> lsStatus = new List<SelectListItem>();
            lsStatus.Add(new SelectListItem() { Text = "Hoạt động", Value = "true" });
            lsStatus.Add(new SelectListItem() { Text = "Khóa", Value = "false" });
            ViewBag.lsStatus = lsStatus;
            return View(models);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Tạo danh mục sản phẩm mới";
            ViewBag.IsEdit = false;
            var model = new CategoryProduct()
            {
                Id = 0,

            };
            return View("Edit", model);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin danh mục sản phẩm";
            ViewBag.IsEdit = true;
            var model = _dbContext.CategoryProducts.Where(p => p.Id == id).FirstOrDefault();
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }


        [HttpPost] 
        public IActionResult Save(CategoryProduct model) 
        {
            //TODO: Kiểm soát dữ liệu trong model xem có hợp lệ hay không?
           
            if (string.IsNullOrWhiteSpace(model.Name))
                ModelState.AddModelError("Name", "Tiêu đề không được để trống");
           

            if (!ModelState.IsValid)
            {
                ViewBag.Title = model.Id == 0 ? "Tạo danh mục sản phẩm mới" : "Cập nhật thông tin danh mục sản phẩm";
                return View("Edit", model);
            }

            if (model.Id == 0)
            {
                var existingCategory = _dbContext.CategoryProducts.FirstOrDefault(c => c.Name == model.Name);
                if (existingCategory != null)
                {
                    ModelState.AddModelError("Name", "Tiêu đề bị trùng");
                    ViewBag.Title = CREATE_TITLE;
                    return View("Edit", model);
                }
                var ls = new CategoryProduct
                {
                    Id = model.Id,
                    Name = model.Name,
                    Description = model.Description,
                    CreateDate = DateTime.Now,
                    IsActive = model.IsActive
                };
                _dbContext.CategoryProducts.Add(ls);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                var catUpdate = _dbContext.CategoryProducts.FirstOrDefault(p => p.Id == model.Id);
                var existingCategory = _dbContext.CategoryProducts.FirstOrDefault(c => c.Id != catUpdate.Id && c.Name == model.Name);
                if (existingCategory != null)
                {
                    ModelState.AddModelError("Error", "Không cập nhật được danh mục. Có thể tiêu đề bị trùng.");
                    ViewBag.Title = "Cập nhật thông tin danh mục sản phẩm";
                    return View("Edit", model);
                }
                catUpdate.Name = model.Name;
                catUpdate.Description = model.Description;
                catUpdate.IsActive = model.IsActive;
                _dbContext.CategoryProducts.Update(catUpdate);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
          
        }

        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                var cat = _dbContext.CategoryProducts.Find(id);
                if (cat == null)
                    return NotFound();
                _dbContext.CategoryProducts.Remove(cat);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            var catIsUsed = _dbContext.Products.Where(c => c.CategoryProductId == id).FirstOrDefault();
            ViewBag.IsUsed = catIsUsed;
            var model = _dbContext.CategoryProducts.Where(p => p.Id == id).FirstOrDefault();
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }
    }
}
