using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AgriculturalForum.Web.Models;
using AgriculturalForum.Web.ModelViews;
using AgriculturalForum.Web.Extensions;
using AgriculturalForum.Web.Helper;

namespace AgriculturalForum.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly KltnDbContext _dbContext;
        private LanguageService _localization;
        public AccountController(KltnDbContext dbContext, LanguageService localization)
        {
            _dbContext = dbContext;
            _localization = localization;
        }
        public IActionResult Index()
        {
            return View();
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


                    if (existingUserWithEmail != null)
                    {
                        ModelState.AddModelError("Email", _localization.Getkey("EmailExist"));
                        return View(account);
                    }

                    if (existingUserWithPhone != null)
                    {
                        ModelState.AddModelError("Phone", _localization.Getkey("PhoneExist"));
                        return View(account);
                    }
                    User user = new User
                    {
                        FullName = account.FullName,
                        Phone = account.Phone.Trim(),
                        Email = account.Email.Trim(),
                        Password = account.Password.ToMD5(),
                        /*  Address = account.Address,*/
                        IsActive = true,
                        IsAdmin = false,
                        ProfileImage = "nophoto.png",
                        Birthday = new DateTime(1990, 1, 1),
                        MemberSince = DateTime.Now
                    };

                    _dbContext.Users.Add(user);
                    await _dbContext.SaveChangesAsync();
                    //Lưu Session
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    var accountID = HttpContext.Session.GetString("UserId");

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


                    if (user == null)
                    {
                        ModelState.AddModelError("Username", _localization.Getkey("EmailDoesNotExist"));
                        return View(account);
                    }

                    string pass = account.Password.ToMD5();
                    if (user.Password != pass)
                    {
                        ModelState.AddModelError("Password", _localization.Getkey("IncorrectPassword"));
                        return View(account);
                    }

                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    var accountID = HttpContext.Session.GetString("UserId");


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

            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var account = _dbContext.Users.Find(Convert.ToInt32(userId));
            if (account == null) return RedirectToAction("Login", "Account");
            if (!string.IsNullOrEmpty(model.CurrentPassword) && model.CurrentPassword.ToMD5() != account.Password)
            {
                ModelState.AddModelError("CurrentPassword", _localization.Getkey("IncorrectCurrentPassword"));

            }

            if (!string.IsNullOrEmpty(model.NewPassword) && model.NewPassword.ToMD5() == account.Password)
            {
                ModelState.AddModelError("NewPassword", _localization.Getkey("CompareCurrentAndNewPassword"));

            }

            if (ModelState.IsValid)
            {

                string passnew = model.NewPassword.Trim().ToMD5();
                account.Password = passnew;
                _dbContext.Users.Update(account);
                _dbContext.SaveChanges();
                return RedirectToAction("Detail", "Account");
            }
            return View();
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

        [HttpGet]
        public IActionResult Update(int id = 0)
        {
            var model = _dbContext.Users
                 .AsNoTracking()
                 .FirstOrDefault(x => x.Id == id);

            return PartialView("_UpdateModalPartial", model);
        }

        [HttpPost]
        public IActionResult Update(User model, IFormFile? Image)
        {
            var account = _dbContext.Users.Find(model.Id);
            if (account == null)
                return NotFound();
            if (string.IsNullOrWhiteSpace(model.FullName))
                ModelState.AddModelError(nameof(model.FullName), _localization.Getkey("FullNameRequired"));
            if (string.IsNullOrWhiteSpace(model.Address))
                ModelState.AddModelError(nameof(model.Address), _localization.Getkey("AddressRequired"));
            if (string.IsNullOrWhiteSpace(model.Province))
                ModelState.AddModelError(nameof(model.Province), _localization.Getkey("ProvinceRequired"));
            if (IsEmailExists(model.Email, model.Id))
            {
                ModelState.AddModelError("Email", _localization.Getkey("EmailExist"));
            }

            if (IsPhoneExists(model.Phone, model.Id))
            {
                ModelState.AddModelError("Phone", _localization.Getkey("PhoneExist"));
            }

            if (ModelState.IsValid)
            {
                account.FullName = model.FullName;
                account.Email = model.Email;
                account.Phone = model.Phone;
                account.Birthday = model.Birthday;
                account.Address = model.Address;
                account.Province = model.Province;
                if (Image != null && Image.Length > 0)
                {
                    // Xóa hình ảnh cũ
                    string oldImagePath = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"uploads/userImages", account.ProfileImage);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                    string extension = Path.GetExtension(Image.FileName);
                    string image = $"user_{DateTime.Now.Ticks}" + extension;
                    string fileName = ApplicationContext.UploadFile(Image, @"uploads/userImages", image);
                    account.ProfileImage = fileName;
                }
                _dbContext.Users.Update(account);
                _dbContext.SaveChanges();
                /*return RedirectToAction("Detail");*/
            }
            return PartialView("_UpdateModalPartial", model);
        }

        private bool IsEmailExists(string email, int userId)
        {
            return _dbContext.Users.Any(x => x.Email == email && x.Id != userId);
        }

        private bool IsPhoneExists(string phone, int userId)
        {
            return _dbContext.Users.Any(x => x.Phone == phone && x.Id != userId);
        }

    }
}
