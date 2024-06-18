using AgriculturalForum.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriculturalForum.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(AuthenticationSchemes ="AdminCookie")]
    [Authorize(Roles ="admin,employee")]
	public class HomeController : Controller
	{
		private readonly KltnDbContext _dbContext;
        public HomeController(KltnDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
		{
			var posts = _dbContext.Posts.Count();
			ViewBag.TotalPosts = posts;
            var products = _dbContext.Products.Count();
            ViewBag.TotalProducts = products;
            var users = _dbContext.Users.Count();
            ViewBag.TotalUsers = users;
            return View();
		}
	}
}
