using AgriculturalForum.Web.Extensions;
using AgriculturalForum.Web.Interfaces;
using AgriculturalForum.Web.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using PagedList.Core;

namespace AgriculturalForum.Web.Controllers
{
    public class ShopController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryProductRepository _categoryProductRepository;
        private readonly IUserRepository _userRepository;
        private LanguageService _localization;
        private readonly INotyfService _notifyService;
        const string CREATE_TITLE = "NewProduct";
        const string EDIT_TITLE = "UpdateProduct";
        public ShopController(IProductRepository productRepository, ICategoryProductRepository categoryProductRepository, IUserRepository userRepository, LanguageService localization, INotyfService notifyService)
        {
            _productRepository = productRepository;
            _categoryProductRepository = categoryProductRepository;
            _userRepository = userRepository;
            _localization = localization;
            _notifyService = notifyService;
        }

        public async Task<IActionResult> Index(int page = 1, int id = 0, string searchValue = "")
        {
            var pageNumber = page;
            var pageSize = 8;
            var lsProducts = await _productRepository.ListOfProducts(id, searchValue);
            PagedList<Product> models = new PagedList<Product>(lsProducts.AsQueryable(), pageNumber, pageSize);

            var categories = await _categoryProductRepository.GetAll();
            ViewBag.ListOfCategories = categories;
            ViewBag.CurrentCateID = id;

            var categoryById = await _categoryProductRepository.GetById(id);
            if (categoryById != null)
                ViewBag.CateName = categoryById.Name;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.SearchValue = searchValue;

            return View(models);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var product = await _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            var similarProducts = await _productRepository.GetSimilarProducts(product.CategoryProductId, id);
            ViewBag.similarProducts = similarProducts;
            return View(product);
        }

        public async Task<IActionResult> Create()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account", new { ReturnUrl = "/Shop/Create" });
           
            var user = await _userRepository.GetUserById(int.Parse(userId));
            if (user != null && (string.IsNullOrEmpty(user.Address) || string.IsNullOrEmpty(user.Province)))
                return RedirectToAction("Detail", "Account");
            ViewBag.Title = _localization.Getkey(CREATE_TITLE);
            var model = new Product()
            {
                Id = 0,
                IsSelling = true
            };
            return View("Edit", model);
        }

        public async Task<IActionResult> Edit(int id = 0)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account", new { ReturnUrl = "/Shop/Edit" });
            ViewBag.Title = _localization.Getkey(EDIT_TITLE);
          
            var model = await _productRepository.GetById(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }

        [HttpPost] //Attribute => chỉ nhân dữ liệu gửi lên dưới dạng Post
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(Product model, IFormFile imgfile, List<IFormFile> additionalImage)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");
          
            var account = await _productRepository.GetUserById(int.Parse(userId));
            if (account == null)
                return NotFound();

            model.UserId = account.Id;
            //TODO: Kiểm soát dữ liệu trong model xem có hợp lệ hay không?

            if (string.IsNullOrWhiteSpace(model.Name))
                ModelState.AddModelError(nameof(model.Name), _localization.Getkey("ProductNameRequired"));
            if (model.Price < 0 || model.Price == null)
                ModelState.AddModelError(nameof(model.Price), _localization.Getkey("PriceValidation"));
            if (model.CategoryProductId == null)
                ModelState.AddModelError(nameof(model.CategoryProductId), _localization.Getkey("CategoryRequired"));

            if (!ModelState.IsValid)
            {
                ViewBag.Title = model.Id == 0 ? _localization.Getkey(CREATE_TITLE) : _localization.Getkey(EDIT_TITLE);
                return View("Edit", model);
            }

            if (model.Id == 0)
            {
                if (imgfile == null || imgfile.Length == 0)
                {
                    ModelState.AddModelError("imgfile", _localization.Getkey("ImageRequired"));
                    ViewBag.Title = CREATE_TITLE;
                    return View("Edit", model);
                }
                else
                {
                    int id = await _productRepository.Add(model, imgfile, additionalImage);
                    if (id == -1)
                    {
                        ViewBag.Title = _localization.Getkey(CREATE_TITLE);
                        return View("Edit", model);
                    }
                    _notifyService.Success(_localization.Getkey("CreateProductSuccess"));
                    return RedirectToAction("Index");
                }
            }
            else
            {
                bool result = await _productRepository.Update(model, imgfile, additionalImage);
                if (!result)
                {
                    ViewBag.Title = _localization.Getkey(EDIT_TITLE);
                    return View("Edit", model);
                }
                _notifyService.Success(_localization.Getkey("UpdateProductSuccess"));
                return RedirectToAction("Index", "User", new { id = model.UserId });
            }
            /* return View("Edit", model);*/
        }



        public async Task<IActionResult> Delete(int id = 0)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var model = await _productRepository.GetById(id);
            if (model == null)
                return View();
            if (Request.Method == "POST")
            {
                await _productRepository.Delete(id);
                _notifyService.Success(_localization.Getkey("DeleteProductSuccess"));
                return RedirectToAction("MyPost", new { id = model.UserId });
            }
            return View(model);

        }

        public async Task<IActionResult> MyPost(int id, int page = 1)
        {
            var pageNumber = page;
            var pageSize = 8;
            var user = await _userRepository.GetUserById(id);
            ViewBag.User = user;
            var lsProducts = await _productRepository.GetProductsByUserId(id);
            PagedList<Product> models = new PagedList<Product>(lsProducts.AsQueryable(), pageNumber, pageSize);
            return View(models);
        }
    }
}
