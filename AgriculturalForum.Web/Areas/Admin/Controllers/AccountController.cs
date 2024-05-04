using AgriculturalForum.Web.Models;
using AgriculturalForum.Web.ModelViews;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AgriculturalForum.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly KltnDbContext _dbContext;
        public AccountController(KltnDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            var userId = HttpContext.Session.GetString("AccountId");
            if (userId != null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email = "", string password = "", string? ReturnUrl = null)
        {
            ViewBag.Email = email;
            if (ModelState.IsValid)
            {
                var user = _dbContext.Users.AsNoTracking().SingleOrDefault(x => x.Email.Trim() == email && x.IsAdmin == true);
                if (user == null)
                {
                    ModelState.AddModelError("Username", "Email không hợp lệ.");
                    return View();
                }

                string pass = password.ToMD5();
                if (user.Password != pass)
                {
                    ModelState.AddModelError("Password", "Mật khẩu không đúng.");
                    return View();
                }

                HttpContext.Session.SetString("AccountId", user.Id.ToString());
                var accountID = HttpContext.Session.GetString("AccountId");


                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.FullName),
                        new Claim("AccountId", user.Id.ToString())
                    };
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync("AdminCookie", claimsPrincipal);
                if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                {
                    return Redirect(ReturnUrl);
                }
                else
                {

                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }


        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync("AdminCookie");
            return RedirectToAction("Login");
        }
    }
}
