using AgriculturalForum.Web.Interfaces;
using AgriculturalForum.Web.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PagedList.Core;

namespace AgriculturalForum.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminCookie", Roles ="admin")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly INotyfService _notifyService;
        const int PAGE_SIZE = 5;
        public UserController(IUserRepository userRepository, INotyfService notifyService)
        {
            _userRepository = userRepository;
            _notifyService = notifyService;
        }
        public async Task<IActionResult> Index(int page = 1, bool? isActive = null, string searchValue = "")
        {
            var pageNumber = page;
            var pageSize = PAGE_SIZE;

            var lsUsers = await _userRepository.ListOfUsers(isActive, searchValue);
            PagedList<User> models = new PagedList<User>(lsUsers.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            ViewBag.IsAcTive = isActive;
            ViewBag.SearchValue = searchValue;
            return View(models);
        }

        public async Task<IActionResult> Detail(int id = 0)
        {
            var model = await _userRepository.GetUserById(id);
            return View(model);
        }

        public async Task<IActionResult> Edit(int id = 0)
        {
            var model = await _userRepository.GetUserById(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Save(User model)
        {
            bool result = await _userRepository.Update(model);
            if (!result)
            {
                _notifyService.Error("Cập nhật trạng thái người dùng không thành công.");
                return View("Edit", model);
            }
            _notifyService.Success("Cập nhật trạng thái người dùng thành công.");
            return RedirectToAction("Index");
        }
    }
}
