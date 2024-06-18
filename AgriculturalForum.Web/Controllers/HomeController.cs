using AgriculturalForum.Web.Extensions;
using AgriculturalForum.Web.Interfaces;
using AgriculturalForum.Web.Models;
using AgriculturalForum.Web.ModelViews;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AgriculturalForum.Web.Controllers
{
    public class HomeController : Controller
	{
        private readonly ILogger<HomeController> _logger;
        private readonly IPostRepository _postRepository;
        private readonly IProductRepository _productRepository;
        private LanguageService _localization;


        public HomeController(ILogger<HomeController> logger, IPostRepository postRepository,
            IProductRepository productRepository, LanguageService localization)
        {
            _logger = logger;
            _postRepository = postRepository;
            _productRepository = productRepository;
            _localization = localization;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.IsHomePage = true;
            HomeViewModel model = new HomeViewModel();


            var allPosts = await _postRepository.GetAll();
            var modelPosts = allPosts.Take(6).ToList();

            var allProducts = await _productRepository.GetAll();
            var modelProducts = allProducts.Take(4).ToList();
            model.Post = modelPosts;
            model.Products = modelProducts;
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

        public IActionResult ChangeLanguage(string culture)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)), new CookieOptions()
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1)
                });
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
