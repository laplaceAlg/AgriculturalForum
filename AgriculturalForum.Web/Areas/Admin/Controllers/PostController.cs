using AgriculturalForum.Web.Extensions;
using AgriculturalForum.Web.Interfaces;
using AgriculturalForum.Web.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace AgriculturalForum.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminCookie")]
    public class PostController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IPostRepository _postRepository;
        private readonly INotyfService _notifyService;

        public PostController(IUserRepository userRepository, IPostRepository postRepository, INotyfService notifyService)
        {
            _userRepository = userRepository;
            _postRepository = postRepository;
            _notifyService = notifyService;
        }
        public async Task<IActionResult> Index(int page = 1, int id = 0)
        {
            var user = await _userRepository.GetUserById(id);
            if (user != null)
                ViewBag.UserName = user.FullName;

            var pageNumber = page;
            var pageSize = 5;


            var lsPosts = await _postRepository.GetPostsByUserId(id);
            PagedList<Post> models = new PagedList<Post>(lsPosts.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentCateID = id;
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

    }
}
