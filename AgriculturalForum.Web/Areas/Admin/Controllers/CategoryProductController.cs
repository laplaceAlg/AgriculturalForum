using AgriculturalForum.Web.Interfaces;
using AgriculturalForum.Web.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PagedList.Core;

namespace AgriculturalForum.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminCookie")]
    [Authorize(Roles = "admin,employee")]
    public class CategoryProductController : Controller
    {
        private readonly ICategoryProductRepository _categoryProductRepository;
        private readonly INotyfService _notifyService;
        const int PAGE_SIZE = 3;
        const string CREATE_TITLE = "Tạo danh mục sản phẩm mới";
        public CategoryProductController(ICategoryProductRepository categoryProductRepository, INotyfService notifyService)
        {
            _categoryProductRepository = categoryProductRepository;
            _notifyService = notifyService;
        }

        public async Task<IActionResult> Index(int page = 1, bool? isActive = null, string searchValue = "")
        {
            var pageNumber = page;
            var pageSize = PAGE_SIZE;

            var lsCategories = await _categoryProductRepository.GetCategoryProducts(isActive, searchValue);

            PagedList<CategoryProduct> models = new PagedList<CategoryProduct>(lsCategories.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            ViewBag.IsActive = isActive;
            ViewBag.SearchValue = searchValue;
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

        public async Task<IActionResult> Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin danh mục sản phẩm";
            ViewBag.IsEdit = true;
            var model = await _categoryProductRepository.GetById(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Save(CategoryProduct model)
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
                int id = await _categoryProductRepository.Add(model);
                if (id == -1)
                {
                    ModelState.AddModelError("Name", "Tiêu đề bị trùng");
                    ViewBag.Title = CREATE_TITLE;
                    return View("Edit", model);
                }
                _notifyService.Success("Tạo danh mục sản phẩm mới thành công.");
                return RedirectToAction("Index");
            }
            else
            {
                bool result = await _categoryProductRepository.Update(model);
                if (!result)
                {
                    ModelState.AddModelError("Error", "Không cập nhật được danh mục. Có thể tiêu đề bị trùng.");
                    ViewBag.Title = "Cập nhật thông tin danh mục sản phẩm";
                    return View("Edit", model);
                }
                _notifyService.Success("Cập nhật thông tin danh mục sản phẩm thành công.");
                return RedirectToAction("Index");
            }

        }

        public async Task<IActionResult> Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                await _categoryProductRepository.Delete(id);
                _notifyService.Success("Xóa danh mục sản phẩm thành công.");
                return RedirectToAction("Index");
            }
            var catIsUsed = await _categoryProductRepository.IsUsed(id);
            ViewBag.IsUsed = catIsUsed;
            var model = await _categoryProductRepository.GetById(id);
            return View(model);
        }
    }
}
