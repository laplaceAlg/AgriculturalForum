using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using AgriculturalForum.Web.Models;
using AgriculturalForum.Web.ModelViews;

namespace AgriculturalForum.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly KltnDbContext _dbcontext;

		public HomeController(ILogger<HomeController> logger, KltnDbContext dbcontext)
		{
			_logger = logger;
			_dbcontext = dbcontext;
		}

		public IActionResult Index()
		{
			ViewBag.IsHomePage = true;
			HomeViewModel model = new HomeViewModel();

			var lsPosts = _dbcontext.Posts
				.AsNoTracking()
				.Include(u => u.User)
				.OrderByDescending(x => x.CreateDate)
				.Take(6)
				.ToList();

			var lsProducts = _dbcontext.Products.AsNoTracking()
				.Where(x => x.IsSelling == true)
				.Include(u => u.User)
				.OrderByDescending(x => x.CreateDate)
				.Take(4)
				.ToList();

			
			model.Post = lsPosts;
			model.Products = lsProducts;
			return View(model);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
