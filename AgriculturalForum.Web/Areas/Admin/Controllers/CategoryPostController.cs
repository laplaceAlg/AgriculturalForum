using AgriculturalForum.Web.Interfaces;
using AgriculturalForum.Web.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PagedList.Core;

namespace AgriculturalForum.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes ="AdminCookie")]
    [Authorize(Roles = "admin,employee")]
    public class CategoryPostController : Controller
    {
        private readonly INotyfService _notifyService;
        const int PAGE_SIZE = 3;
        const string CREATE_TITLE = "Tạo danh mục bài viết mới";
        private readonly ICategoryPostRepository _categoryPostRepository;
        public CategoryPostController(INotyfService notifyService, ICategoryPostRepository categoryPostRepository)
        {
            _notifyService = notifyService;
            _categoryPostRepository = categoryPostRepository;
        }

        public async Task<IActionResult> Index(int page = 1, bool? isActive = null, string searchValue = "")
        {
            var pageNumber = page;
            var pageSize = PAGE_SIZE;

            var lsCategories = await _categoryPostRepository.GetCategoryPosts(isActive, searchValue);

            PagedList<CategoryPost> models = new PagedList<CategoryPost>(lsCategories.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            ViewBag.IsActive = isActive;
            ViewBag.SearchValue = searchValue;
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
            var model = _categoryPostRepository.GetById(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }

        [HttpPost] //Attribute => chỉ nhân dữ liệu gửi lên dưới dạng Post
        public async Task<IActionResult> Save(CategoryPost model)
        {
            //TODO: Kiểm soát dữ liệu trong model xem có hợp lệ hay không?

            if (string.IsNullOrWhiteSpace(model.Title))
                ModelState.AddModelError("Name", "Tiêu đề không được để trống");


            if (!ModelState.IsValid)
            {
                ViewBag.Title = model.Id == 0 ? "Tạo danh mục bài viết mới" : "Cập nhật thông tin danh mục bài viết";
                return View("Edit", model);
            }

            if (model.Id == 0)
            {

                int id = await _categoryPostRepository.Add(model);
                if (id == -1)
                {

                    ModelState.AddModelError("Name", "Tiêu đề bị trùng");
                    ViewBag.Title = CREATE_TITLE;
                    return View("Edit", model);
                }
                _notifyService.Success("Tạo danh mục bài viết mới thành công.");
                return RedirectToAction("Index");
            }
            else
            {
                bool result = await _categoryPostRepository.Update(model);
                if (!result)
                {
                    ModelState.AddModelError("Error", "Không cập nhật được danh mục. Có thể tiêu đề bị trùng.");
                    ViewBag.Title = "Cập nhật thông tin danh mục bài viết";
                    return View("Edit", model);
                }
                _notifyService.Success("Cập nhật thông tin danh mục bài viết thành công.");
                return RedirectToAction("Index");
            }

        }

        public async Task<IActionResult> Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                await _categoryPostRepository.Delete(id);
                _notifyService.Success("Xóa danh mục bài viết thành công.");
                return RedirectToAction("Index");
            }
            var catIsUsed = await _categoryPostRepository.IsUsed(id);
            ViewBag.IsUsed = catIsUsed;
            var model = await _categoryPostRepository.GetById(id);
            return View(model);
        }
    }
}
