using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using AgriculturalForum.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace AgriculturalForum.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes ="AdminCookie")]
    public class CategoryPostController : Controller
    {
        private readonly KltnDbContext _dbContext;
        const int PAGE_SIZE = 3;
        const string CREATE_TITLE = "Tạo danh mục bài viết mới";
        public CategoryPostController(KltnDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index(int page = 1, bool? isActive = null, string searchValue = "")
        {
            var pageNumber = page;
            var pageSize = PAGE_SIZE;

          
            List<CategoryPost> lsCategories = new List<CategoryPost>();
            lsCategories = _dbContext.CategoryPosts
                   .AsNoTracking()
                   .OrderByDescending(x => x.CreateDate)
                   .ToList();
           

            if (!string.IsNullOrEmpty(searchValue) && isActive.HasValue)
            {
                lsCategories = _dbContext.CategoryPosts
                        .AsNoTracking()
                        .Where(x => x.IsActive == isActive && x.Title.Contains(searchValue))
                        .OrderByDescending(x => x.CreateDate)
                        .ToList();
            }
            else if(!string.IsNullOrEmpty(searchValue))
            {
                lsCategories = _dbContext.CategoryPosts
                      .AsNoTracking()
                      .Where(x => x.Title.Contains(searchValue))
                      .OrderByDescending(x => x.CreateDate)
                      .ToList();
            }
            else if(isActive.HasValue)
            {

                lsCategories = _dbContext.CategoryPosts
                    .AsNoTracking()
                    .Where(x => x.IsActive == isActive)
                    .OrderByDescending(x => x.CreateDate)
                    .ToList();
            }


            PagedList<CategoryPost> models = new PagedList<CategoryPost>(lsCategories.AsQueryable(), pageNumber, pageSize);

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
            ViewBag.Title = "Tạo danh mục bài viết mới";
            ViewBag.IsEdit = false;
            var model = new CategoryPost()
            {
                Id = 0,

            };
            return View("Edit", model);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin danh mục bài viết";
            ViewBag.IsEdit = true;
            var model = _dbContext.CategoryPosts.Where(p => p.Id == id).FirstOrDefault();
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }

        [HttpPost] //Attribute => chỉ nhân dữ liệu gửi lên dưới dạng Post
        public IActionResult Save(CategoryPost model)
        {
            //TODO: Kiểm soát dữ liệu trong model xem có hợp lệ hay không?

            if (string.IsNullOrWhiteSpace(model.Title))
                ModelState.AddModelError("Name", "Tiêu đề không được để trống");


            if (!ModelState.IsValid)
            {
                ViewBag.Title = model.Id == 0 ? "Tạo danh mục sản phẩm mới" : "Cập nhật thông tin danh mục sản phẩm";
                return View("Edit", model);
            }

            if (model.Id == 0)
            {
                var existingCategory = _dbContext.CategoryPosts.FirstOrDefault(c => c.Title == model.Title);
                if (existingCategory != null)
                {
                    ModelState.AddModelError("Name", "Tiêu đề bị trùng");
                    ViewBag.Title = CREATE_TITLE;
                    return View("Edit", model);
                }
                var ls = new CategoryPost
                {
                    Id = model.Id,
                    Title = model.Title,
                    Description = model.Description,
                    CreateDate = DateTime.Now,
                    IsActive = model.IsActive
                };
                _dbContext.CategoryPosts.Add(ls);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {

                var catUpdate = _dbContext.CategoryPosts.FirstOrDefault(c => c.Id == model.Id);
                var existingCategory = _dbContext.CategoryPosts.FirstOrDefault(c => c.Id != catUpdate.Id && c.Title == model.Title);
                if (existingCategory != null)
                {
                    ModelState.AddModelError("Error", "Không cập nhật được danh mục. Có thể tiêu đề bị trùng.");
                    ViewBag.Title = "Cập nhật thông tin danh mục bài viết";
                    return View("Edit", model);
                }
                catUpdate.Title = model.Title;
                catUpdate.Description = model.Description;
                catUpdate.IsActive = model.IsActive;
                _dbContext.CategoryPosts.Update(catUpdate);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }

        }

        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                var cat = _dbContext.CategoryPosts.Find(id);
                if (cat == null)
                    return NotFound();
                _dbContext.CategoryPosts.Remove(cat);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            var catIsUsed = _dbContext.Posts.Where(c => c.CategoryPostId == id).FirstOrDefault();
            ViewBag.IsUsed = catIsUsed;
            var model = _dbContext.CategoryPosts.Where(p => p.Id == id).FirstOrDefault();
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }
    }
}
