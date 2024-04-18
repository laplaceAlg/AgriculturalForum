﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System.Drawing.Printing;
using System.Security.Permissions;
using AgriculturalForum.Web.Models;

namespace AgriculturalForum.Web.Areas.Admin.Controllers
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
        public IActionResult Index(int page = 1, bool? isActive = null, string searchValue = "")
        {
            var pageNumber = page;
            var pageSize = PAGE_SIZE;

            List<User> lsUsers = new List<User>();

            lsUsers = _dbContext.Users
            .AsNoTracking()
            .OrderByDescending(x => x.MemberSince).ToList();
      
            if (!string.IsNullOrEmpty(searchValue) && isActive.HasValue)
            {
                lsUsers = _dbContext.Users
                        .AsNoTracking()
                        .Where(x => x.IsActive == isActive && (x.FullName.Contains(searchValue) || x.Email.Contains(searchValue)))
                        .OrderByDescending(x => x.MemberSince)
                        .ToList();
            }
            else if (!string.IsNullOrEmpty(searchValue))
            {
                lsUsers = _dbContext.Users
                      .AsNoTracking()
                      .Where(x => x.FullName.Contains(searchValue) || x.Email.Contains(searchValue))
                      .OrderByDescending(x => x.MemberSince)
                      .ToList();
            }
            else if (isActive.HasValue)
            {

                lsUsers = _dbContext.Users
                    .AsNoTracking()
                    .Where(x => x.IsActive == isActive)
                    .OrderByDescending(x => x.MemberSince)
                    .ToList();
            }


            PagedList<User> models = new PagedList<User>(lsUsers.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            ViewBag.IsAcTive = isActive;
            ViewBag.SearchValue = searchValue;
            List<SelectListItem> lsStatus = new List<SelectListItem>();
            lsStatus.Add(new SelectListItem() { Text = "Hoạt động", Value = "true" });
            lsStatus.Add(new SelectListItem() { Text = "Khóa", Value = "false" });
            ViewBag.lsStatus = lsStatus;
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