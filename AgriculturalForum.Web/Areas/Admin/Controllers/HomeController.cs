using Microsoft.AspNetCore.Mvc;

namespace AgriculturalForum.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
