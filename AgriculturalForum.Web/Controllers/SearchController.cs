using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using AgriculturalForum.Web.Models;

namespace AgriculturalForum.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly KltnDbContext _dbContext;
        const int PAGE_SIZE_POST = 5;
        const int PAGE_SIZE_PRODUCT = 8;
        public SearchController(KltnDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index(int page = 1, string searchValue = "")
        {
            var pageNumber = page;
            var pageSize = 6;
         
            List<Post> lsPost = new List<Post>();
            lsPost = _dbContext.Posts
            .AsNoTracking()
            .Where(x => x.Title.Contains(searchValue) || x.Content.Contains(searchValue))
            .Include(x => x.User)
            .Include(x => x.PostReplies)
            .OrderBy(x => x.Id).ToList();
            PagedList<Post> models = new PagedList<Post>(lsPost.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            ViewBag.SearchValue = searchValue;
            return View(models);
        }


        public IActionResult ListPosts(int id = 0, int page = 1)
        {
            var pageNumber = page;
            var pageSize = PAGE_SIZE_POST;

            var models = _dbContext.Posts
                .AsNoTracking()
                .Where(p => p.UserId == id)
                .Include(p => p.PostReplies)
                .Include(p => p.User)
                .OrderByDescending(p => p.CreateDate).ToList();
            PagedList<Post> modelPost = new PagedList<Post>(models.AsQueryable(), pageNumber, pageSize);



            return View(modelPost);
        }

        public IActionResult ListProducts(int id = 0, int page = 1)
        {
            var pageNumber = page;
            var pageSize = PAGE_SIZE_PRODUCT;

            var models = _dbContext.Products
                .AsNoTracking()
                .Where(p => p.UserId == id)
                .Include(p => p.ProductImages)
                .Include(p => p.User)
                .OrderByDescending(p => p.IsSelling).ToList();
            PagedList<Product> modelProducts = new PagedList<Product>(models.AsQueryable(), pageNumber, pageSize);
            ViewBag.UserId = id;
            return View(modelProducts);
        }
    }
}
