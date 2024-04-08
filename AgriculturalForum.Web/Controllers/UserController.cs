using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System.Runtime.InteropServices.JavaScript;
using AgriculturalForum.Web.Models;

namespace AgriculturalForum.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly KltnDbContext _dbcontext;

        public UserController(KltnDbContext context)
        {
            _dbcontext = context;
        }

        public IActionResult Index(int id = 0)
        {
            var model = _dbcontext.Users
                .Where(u => u.Id == id)
                .FirstOrDefault();
            var lsPosts = _dbcontext.Posts.Where(p => p.UserId == id)
                                          .Include(p => p.PostReplies)
                                          .OrderByDescending(p  => p.CreateDate).Take(4).ToList();
            ViewBag.ListPosts = lsPosts;
            var lsProducts = _dbcontext.Products.Where(p => p.UserId == id)
                                                .OrderByDescending(p => p.CreateDate).Take(4).ToList(); 
            ViewBag.ListProducts = lsProducts;
            ViewBag.TotalPost = _dbcontext.Posts.Where(p => p.UserId == id).Count();
            ViewBag.TotalProduct = _dbcontext.Products.Where(p => p.UserId == id).Count();
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

    }
}
