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
    public class ProductController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly INotyfService _notifyService;

        public ProductController(IUserRepository userRepository, IProductRepository productRepository, INotyfService notifyService)
        {
            _userRepository = userRepository;
            _productRepository = productRepository;
            _notifyService = notifyService;
        }
        public async Task<IActionResult> Index(int page = 1, int id = 0)
        {
            var user = await _userRepository.GetUserById(id);
            if (user != null)
                ViewBag.UserName = user.FullName;

            var pageNumber = page;
            var pageSize = 5;

            var lsProducts = await _productRepository.GetProductsByUserId(id);
            PagedList<Product> models = new PagedList<Product>(lsProducts.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentCateID = id;
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

    }
}
