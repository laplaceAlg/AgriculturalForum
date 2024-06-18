using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using AgriculturalForum.Web.Models;
using AgriculturalForum.Web.Interfaces;

namespace AgriculturalForum.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly IProductRepository _productRepository;
        private readonly KltnDbContext _dbContext;
        const int PAGE_SIZE_POST = 5;
        const int PAGE_SIZE_PRODUCT = 8;
        public SearchController(IPostRepository postRepository, IProductRepository productRepository, KltnDbContext dbContext)
        {
            _postRepository = postRepository;
            _productRepository = productRepository;
            _dbContext = dbContext;
        }
        public async Task<IActionResult> Index(int page = 1, string searchValue = "")
        {
            var pageNumber = page;
            var pageSize = 6;

            var lsPosts = await _postRepository.GetPostsBySearchValue(searchValue);
            PagedList<Post> models = new PagedList<Post>(lsPosts.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            ViewBag.SearchValue = searchValue;
            return View(models);
        }


        public async Task<IActionResult> ListPosts(int id = 0, int page = 1)
        {
            var pageNumber = page;
            var pageSize = PAGE_SIZE_POST;
            var models = await _postRepository.GetPostsByUserId(id);
            PagedList<Post> modelPost = new PagedList<Post>(models.AsQueryable(), pageNumber, pageSize);
            return View(modelPost);
        }

        public async Task<IActionResult> ListProducts(int id = 0, int page = 1)
        {
            var pageNumber = page;
            var pageSize = PAGE_SIZE_PRODUCT;

            var models = await _productRepository.GetProductsByUserId(id);
            PagedList<Product> modelProducts = new PagedList<Product>(models.AsQueryable(), pageNumber, pageSize);
            ViewBag.UserId = id;
            return View(modelProducts);
        }
    }
}
