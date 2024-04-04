using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Helpper;

namespace WebApplication1.Controllers
{
    public class ForumController : Controller
    {
        private readonly KltnDbContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ForumController(KltnDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index1()
        {
            var categoriesWithPosts = _dbContext.CategoryPosts
                            .Include(c => c.Posts)
                                 .ThenInclude(p => p.User)
                            .Include(c => c.Posts)
                                   .ThenInclude(p => p.PostReplies)
                            .ToList();
            var recentPosts = _dbContext.Posts
                             .OrderByDescending(p => p.CreateDate) // Sắp xếp theo ngày tạo mới nhất
                             .Take(5)
                             .ToList();
            ViewBag.RecentPosts = recentPosts;

            var totalPosts = _dbContext.Posts.Count();
            var totalPostReplies = _dbContext.PostReplies.Count();
            var totalMembers = _dbContext.Users.Count();
            var latestMember = _dbContext.Users.OrderByDescending(u => u.MemberSince).FirstOrDefault();


            ViewBag.TotalPosts = totalPosts;
            ViewBag.TotalPostReplies = totalPostReplies;
            ViewBag.TotalMembers = totalMembers;
            ViewBag.LatestMember = latestMember;

            return View(categoriesWithPosts);
        }

        public IActionResult Index()
        {
            var latestPostsByCategory = _dbContext.Posts
                 .Include(p => p.User)
                    .GroupBy(p => p.CategoryPostId)
                    .Select(g => g.OrderByDescending(p => p.CreateDate).FirstOrDefault())
                    .ToList();
            ViewBag.LatestPostOfCat = latestPostsByCategory;


            var recentPosts = _dbContext.Posts
                            .OrderByDescending(p => p.CreateDate)
                            .Take(5)
                            .ToList();
            ViewBag.RecentPosts = recentPosts;

            var totalPosts = _dbContext.Posts.Count();
            var totalPostReplies = _dbContext.PostReplies.Count();
            var totalMembers = _dbContext.Users.Count();
            var latestMember = _dbContext.Users.OrderByDescending(u => u.MemberSince).FirstOrDefault();


            ViewBag.TotalPosts = totalPosts;
            ViewBag.TotalPostReplies = totalPostReplies;
            ViewBag.TotalMembers = totalMembers;
            ViewBag.LatestMember = latestMember;
            var model = _dbContext.CategoryPosts
                .Include(p => p.Posts)
                .ThenInclude(pr => pr.PostReplies)
                .ToList();
            return View(model);
        }

        public IActionResult MyPost(int id)
        {
            var myPosts = _dbContext.Users.Where(p => p.Id == id)
                .Include(p => p.Posts)
                .ThenInclude(pr => pr.PostReplies)
               .FirstOrDefault();
            if (myPosts == null)
                return RedirectToAction("Index");
            return View(myPosts);
        }
    }
}
