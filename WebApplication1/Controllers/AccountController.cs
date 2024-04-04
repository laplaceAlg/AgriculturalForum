using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Claims;
using WebApplication1.Models;
using WebApplication1.ModelViews;

namespace WebApplication1.Controllers
{
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

        [AcceptVerbs("GET", "POST")]
        [AllowAnonymous]
        public IActionResult ValidatePhone(string Phone)
        {
            var existingPhone = _dbContext.Users.Any(x => x.Phone.ToLower() == Phone.ToLower());
            if (existingPhone)
                return Json($"Phone {Phone} is already in use.");

            return Json(true);

        }

        [AcceptVerbs("GET", "POST")]
        [AllowAnonymous]
        public IActionResult ValidateEmail(string Email)
        {

            var existingEmail = _dbContext.Users.Any(x => x.Email.ToLower() == Email.ToLower());
            if (existingEmail)
                return Json($"Email {Email} is already in use.");
            return Json(true);
        }



        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel account)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingUserWithEmail = _dbContext.Users.AsNoTracking().FirstOrDefault(x => x.Email.ToLower() == account.Email.ToLower());
                    var existingUserWithPhone = _dbContext.Users.AsNoTracking().FirstOrDefault(x => x.Phone.ToLower() == account.Phone.Trim().ToLower());

                    // Nếu đã tồn tại email hoặc số điện thoại, trả về thông báo lỗi
                    if (existingUserWithEmail != null)
                    {
                        ModelState.AddModelError("Email", $"Email {account.Email} is already in use.");
                        return View(account);
                    }

                    if (existingUserWithPhone != null)
                    {
                        ModelState.AddModelError("Phone", $"Phone {account.Phone} is already in use.");
                        return View(account);
                    }
                    User user = new User
                    {
                        FullName = account.FullName,
                        Phone = account.Phone.Trim().ToLower(),
                        Email = account.Email.Trim().ToLower(),
                        Password = (account.Password).ToMD5(),
                        Address = account.Address,
                        IsActive = true,
                        IsAdmin = false,
                        ProfileImage = @"themes\assets\img\users\nophoto.png",
                        MemberSince = DateTime.Now
                    };
                    try
                    {
                        _dbContext.Users.Add(user);
                        await _dbContext.SaveChangesAsync();
                        //Lưu Session
                        HttpContext.Session.SetString("UserId", user.Id.ToString());
                        var taikhoanID = HttpContext.Session.GetString("UserId");

                        //Identity
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name,user.FullName),
                            new Claim("UserId", user.Id.ToString())
                        };
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(claimsPrincipal);
                        return RedirectToAction("Index", "Forum");
                    }
                    catch
                    {
                        return RedirectToAction("Register");
                    }
                }
                else
                {
                    return View(account);
                }
            }
            catch
            {
                return View(account);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string? ReturnUrl = null)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId != null)
            {
                return RedirectToAction("Index", "Forum");
            }
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel account, string? ReturnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = _dbContext.Users.AsNoTracking().SingleOrDefault(x => x.Email.Trim() == account.UserName);

                    if (user == null) return RedirectToAction("Register");
                    string pass = account.Password.ToMD5();
                    if (user.Password != pass)
                    {
                        return View(account);
                    }


                    //Luu Session MaKh
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    var taikhoanID = HttpContext.Session.GetString("UserId");

                    
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.FullName),
                        new Claim("UserId", user.Id.ToString())
                    };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                       
                        return RedirectToAction("Index", "Forum");
                    }
                }
            }
            catch
            {
                return RedirectToAction("Register");
            }
            return View(account);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("UserId");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ChangePassword()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }


        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");
                if (userId == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                if (ModelState.IsValid)
                {
                    var taikhoan = _dbContext.Users.Find(Convert.ToInt32(userId));
                    if (taikhoan == null) return RedirectToAction("Login", "Account");
                    var pass = model.PasswordNow.Trim().ToMD5();
                    {
                        string passnew = model.Password.Trim().ToMD5();
                        taikhoan.Password = passnew;
                        _dbContext.Users.Update(taikhoan);
                        _dbContext.SaveChanges();
                        return RedirectToAction("Index", "Forum");
                    }
                }
            }
            catch
            {
                return RedirectToAction("Index", "Forum");
            }
            return RedirectToAction("Index", "Forum");
        }

        public IActionResult Detail()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId != null)
            {
                var user = _dbContext.Users.AsNoTracking().SingleOrDefault(x => x.Id == Convert.ToInt32(userId));
                if (user != null)
                {
                    return View(user);
                }

            }
            return RedirectToAction("Login");
        }
     
    }
}
