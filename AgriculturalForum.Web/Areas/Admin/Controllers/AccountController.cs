using AgriculturalForum.Web.Models;
using AgriculturalForum.Web.ModelViews;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace AgriculturalForum.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminCookie")]
    public class AccountController : Controller
    {
        private readonly KltnDbContext _dbContext;
        private readonly INotyfService _notifyService;
        public AccountController(KltnDbContext dbContext, INotyfService notifyService)
        {
            _dbContext = dbContext;
            _notifyService = notifyService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            var userId = HttpContext.Session.GetString("AccountId");
            if (userId != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email = "", string password = "")
        {
            ViewBag.Email = email;
            if (ModelState.IsValid)
            {
                var employee = _dbContext.Employees.AsNoTracking().SingleOrDefault(x => x.Email.Trim() == email && x.IsWorking == true);
                if (employee == null)
                {
                    ModelState.AddModelError("Username", "Email không hợp lệ.");
                    return View();
                    /*  return View("AccessDenied");*/
                }

                if (employee.Password != password)
                {
                    ModelState.AddModelError("Password", "Mật khẩu không đúng.");
                    return View();
                }

                HttpContext.Session.SetString("AccountId", employee.EmployeeId.ToString());
                var accountID = HttpContext.Session.GetString("AccountId");
                var roles = employee.RoleNames.Split(',').ToList();
                
                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, employee.FullName),
                        new Claim("AccountId", employee.EmployeeId.ToString())
                    };
                if (roles != null)
                    foreach (var role in roles)
                        claims.Add(new Claim(ClaimTypes.Role, role));
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync("AdminCookie", claimsPrincipal);
                _notifyService.Success("Đăng nhập thành công");
                return RedirectToAction("Index", "Home");

            }
            return View();
        }


        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("AccountId");
            await HttpContext.SignOutAsync("AdminCookie");
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }


        [HttpPost]
        public IActionResult ChangePassword(string CurrentPassword = "", string NewPassword = "", string ConfirmPassword = "")
        {

            var employeeId = HttpContext.Session.GetString("AccountId");
            if (employeeId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var account = _dbContext.Employees.Find(Convert.ToInt32(employeeId));
            if (account == null) return RedirectToAction("Login", "Account");
            if (string.IsNullOrWhiteSpace(CurrentPassword))
            {
                ModelState.AddModelError("CurrentPassword", "Vui lòng nhập mật khẩu hiện tại");

            }
            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                ModelState.AddModelError("NewPassword", "Vui lòng nhập mật khẩu mới");

            }
            if (string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ModelState.AddModelError("ConfirmPassword", "Vui lòng xác nhận mật khẩu mới");

            }
            if (CurrentPassword != account.Password)
            {
                ModelState.AddModelError("CurrentPassword", "Mật khẩu không chính xác");

            }

            if (NewPassword == account.Password)
            {
                ModelState.AddModelError("NewPassword","Mật khẩu mới không được trùng với mật khẩu cũ");

            }

            if (ConfirmPassword != NewPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Mật khẩu mới và mật khẩu xác nhận không khớp");

            }

            if (ModelState.IsValid)
            {
                account.Password = NewPassword;
                _dbContext.Employees.Update(account);
                _dbContext.SaveChanges();
                _notifyService.Success("Đổi mật khẩu thành công");
                return RedirectToAction("Index", "Home");
            }
            _notifyService.Error("Đổi mật khẩu không thành công");
            return View();
        }

        public IActionResult Profile ()
        {
            var employeeId = HttpContext.Session.GetString("AccountId");
            if (employeeId == null)
            {
                return RedirectToAction("Login", "Account");
            }
           /* var account = _dbContext.Employees.Find(Convert.ToInt32(employeeId));*/
            var model = _dbContext.Employees.Where(p => p.EmployeeId == Convert.ToInt32(employeeId)).FirstOrDefault();
            return View(model);
        }
    }
}
