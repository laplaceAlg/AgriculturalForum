using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System.Drawing.Printing;
using System.Security.Permissions;
using WebApplication1.Models;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly KltnDbContext _dbContext;
        const int PAGE_SIZE = 5;
        public UserController(KltnDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index(int page = 1)
        {
            var pageNumber = page;
            var pageSize = PAGE_SIZE;

            List<User> lsUsers = new List<User>();

            lsUsers = _dbContext.Users
            .AsNoTracking()
            .OrderByDescending(x => x.MemberSince).ToList();
            PagedList<User> models = new PagedList<User>(lsUsers.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        public IActionResult Detail(int id = 0)
        {
            var model = _dbContext.Users.Where(p => p.Id == id).FirstOrDefault();
            return View(model);
        }

        public IActionResult Edit(int id = 0)
        {
            var model = _dbContext.Users.Where(p => p.Id == id).FirstOrDefault();
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }

        [HttpPost]
        public IActionResult Save(User model)
        {
            var userUpdate = _dbContext.Users.FirstOrDefault(u => u.Id == model.Id);
            userUpdate.IsActive = model.IsActive;
            _dbContext.Users.Update(userUpdate);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
