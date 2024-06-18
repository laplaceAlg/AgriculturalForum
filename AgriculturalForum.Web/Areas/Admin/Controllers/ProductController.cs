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
    public class ProductController : Controller
    {
        private readonly KltnDbContext _dbContext;
        private readonly INotyfService _notifyService;

        public ProductController(KltnDbContext dbContext, INotyfService notifyService)
        {
            _dbContext = dbContext;
            _notifyService = notifyService;
        }
        public IActionResult Index(int page = 1, int id = 0)
        {

            ViewBag.UserName = _dbContext.Users
                     .Where(c => c.Id == id)
                     .Select(c => c.FullName)
                     .FirstOrDefault();

            var pageNumber = page;
            var pageSize = 5;

            List<Product> lsProducts = new List<Product>();
            if (id != 0)
            {
                lsProducts = _dbContext.Products
                .AsNoTracking()
                .Where(x => x.CategoryProductId == id)
                .Include(x => x.CategoryProduct)
                .OrderByDescending(x => x.CreateDate).ToList();
            }


            PagedList<Product> models = new PagedList<Product>(lsProducts.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentCateID = id;
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

    }
}
