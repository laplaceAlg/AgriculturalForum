using AgriculturalForum.Web.Extensions;
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
    public class PostController : Controller
    {
        private readonly KltnDbContext _dbContext;
        private readonly INotyfService _notifyService;

        public PostController(KltnDbContext dbContext, INotyfService notifyService)
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

            List<Post> lsPosts = new List<Post>();
            if (id != 0)
            {
                lsPosts = _dbContext.Posts
                .AsNoTracking()
                .Where(x => x.UserId == id)
                .Include(x => x.CategoryPost)
                .Include(x => x.PostReplies)
                .OrderByDescending(x => x.CreateDate).ToList();
            }


            PagedList<Post> models = new PagedList<Post>(lsPosts.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentCateID = id;
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

    }
}
