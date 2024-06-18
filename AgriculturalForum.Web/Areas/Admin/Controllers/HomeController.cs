using AgriculturalForum.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriculturalForum.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(AuthenticationSchemes ="AdminCookie")]
    [Authorize(Roles ="admin,employee")]
	public class HomeController : Controller
	{
        private readonly ICategoryPostRepository _categoryPostRepository;
        private readonly ICategoryProductRepository _categoryProductRepository;
        private readonly IUserRepository _userRepository;
        public HomeController(ICategoryPostRepository categoryPostRepository,
            ICategoryProductRepository categoryProductRepository, IUserRepository userRepository)
        {
            _categoryPostRepository = categoryPostRepository;
            _categoryProductRepository = categoryProductRepository;
            _userRepository = userRepository;
        }
        public async Task<IActionResult> Index()
        {
            var totalPosts = await _categoryPostRepository.GetTotalCategoryPosts();
            ViewBag.TotalPosts = totalPosts;
            var totalProducts = await _categoryProductRepository.GetTotalCategoryProducts();
            ViewBag.TotalProducts = totalProducts;
            var totalUsers = await _userRepository.GetTotalMembers();
            ViewBag.TotalUsers = totalUsers;
            return View();
        }
    }
}
