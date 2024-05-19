using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using AgriculturalForum.Web.Helper;
using AgriculturalForum.Web.Models;
using AgriculturalForum.Web.Extensions;
using AspNetCoreHero.ToastNotification.Abstractions;


namespace AgriculturalForum.Web.Controllers
{
    public class PostController : Controller
    {
        private readonly KltnDbContext _dbContext;
        private LanguageService _localization;
        private readonly INotyfService _notifyService;
        const string CREATE_TITLE = "NewPost";
        const string EDIT_TITLE = "UpdatePost";
        public PostController(KltnDbContext dbContext, LanguageService localization, INotyfService notifyService)
        {
            _dbContext = dbContext;
            _localization = localization;
            _notifyService = notifyService;
        }
        

        public IActionResult Index(int page = 1, int id = 0)
        {

            ViewBag.Title = _dbContext.CategoryPosts
                     .Where(c => c.Id == id)
                     .Select(c => c.Title)
                     .FirstOrDefault();

            var pageNumber = page;
            var pageSize = 3;

            List<Post> lsPosts = new List<Post>();
            if (id != 0)
            {
                lsPosts = _dbContext.Posts
                .AsNoTracking()
                .Where(x => x.CategoryPostId == id )
                .Include(x => x.User)
                .Include(x => x.PostReplies)
                .OrderByDescending(x => x.CreateDate).ToList();
            }


            PagedList<Post> models = new PagedList<Post>(lsPosts.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentCateID = id;
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        public IActionResult Detail(int id = 0)
        {
            var post = _dbContext.Posts
                        .Include(q => q.User)
                        .Include(c => c.PostReplies)
                        .ThenInclude(q => q.User)
                        .FirstOrDefault(m => m.Id == id);

         
            var similarPosts = _dbContext.Posts
                                .Where(p => p.CategoryPostId == post.CategoryPostId && p.Id != id )
                                .OrderByDescending(p => p.CreateDate)
                                .Take(5) 
                                .ToList();

            ViewBag.SimilarPosts = similarPosts;

            return View(post);
        }

        public IActionResult Create()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account", new { ReturnUrl = "/Post/Create" });
            ViewBag.Title = _localization.Getkey(CREATE_TITLE);
            ViewBag.IsEdit = false;
            var model = new Post()
            {
                Id = 0,
                Image = "default.jpg"
            };
            return View("Edit", model);
        }

        public IActionResult Edit(int id = 0)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account", new { ReturnUrl = "/Post/Edit" });
            ViewBag.Title = _localization.Getkey(EDIT_TITLE);
            ViewBag.IsEdit = true;
            var model = _dbContext.Posts.Where(p => p.Id == id).FirstOrDefault();
            if (model == null)
                return RedirectToAction("Index");
            if (string.IsNullOrWhiteSpace(model.Image))
                model.Image = "default.jpg";
            return View(model);
        }

        [HttpPost] 
        [ValidateAntiForgeryToken]
        public IActionResult Save(Post model, IFormFile? imgFile = null)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");
            var account = _dbContext.Users.AsNoTracking().FirstOrDefault(u => u.Id == int.Parse(userId));
            if (account == null) 
                return NotFound();

            //TODO: Kiểm soát dữ liệu trong model xem có hợp lệ hay không?

            if (string.IsNullOrWhiteSpace(model.Title))
                ModelState.AddModelError(nameof(model.Title), _localization.Getkey("TitleRequired"));
            if (string.IsNullOrWhiteSpace(model.Content))
                ModelState.AddModelError(nameof(model.Content), _localization.Getkey("ContentRequired"));
            if (model.CategoryPostId == null)
                ModelState.AddModelError(nameof(model.CategoryPostId), _localization.Getkey("CategoryRequired"));

            if (!ModelState.IsValid)
            {
                ViewBag.Title = model.Id == 0 ? _localization.Getkey(CREATE_TITLE) : _localization.Getkey(EDIT_TITLE);
                ViewBag.IsEdit = model.Id == 0 ? false : true;
                return View("Edit", model);
            }

            if (model.Id == 0)
            {
                if (imgFile != null && imgFile.Length > 0)
                {
                    string extension = Path.GetExtension(imgFile.FileName);
                    string image = $"post_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}" + extension;
                    string fileName = ApplicationContext.UploadFile(imgFile, @"uploads/postImages", image);
                    model.Image = fileName;
                }
                var ls = new Post
                {
                    Id = model.Id,
                    Title = model.Title,
                    Content = model.Content,
                    CreateDate = DateTime.Now,
                    Image = model.Image,
                    CategoryPostId = model.CategoryPostId,
                    UserId = account.Id,
                };
                _dbContext.Posts.Add(ls);
                _dbContext.SaveChanges();
                _notifyService.Success(_localization.Getkey("CreatePostSuccess"));
                return RedirectToAction("Index", new { id = model.CategoryPostId });

            }
            else
            {
                var postToUpdate = _dbContext.Posts.FirstOrDefault(p => p.Id == model.Id);
                if (imgFile != null && imgFile.Length > 0)
                {
                    // Xóa hình ảnh cũ
                    string oldImagePath = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"uploads/postImages", postToUpdate.Image);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                    string extension = Path.GetExtension(imgFile.FileName);
                    string image = $"post_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}" + extension;
                    string fileName = ApplicationContext.UploadFile(imgFile, @"uploads/postImages", image);
                    model.Image = fileName;
                }

                postToUpdate.Title = model.Title;
                postToUpdate.Content = model.Content;
                postToUpdate.CategoryPostId = model.CategoryPostId;
                postToUpdate.Image = model.Image;
                _dbContext.Posts.Update(postToUpdate);
                _dbContext.SaveChanges();
                _notifyService.Success(_localization.Getkey("UpdatePostSuccess"));
                return RedirectToAction("Detail", new { id = model.Id });
            }
            /* return RedirectToAction("Edit");*/
        }

        public IActionResult Delete(int id = 0)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");
            var post = _dbContext.Posts.Find(id);
            if (post.Image != "default.jpg")
            {
                var imagePath = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"uploads/postImages", post.Image);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            var postReplies = _dbContext.PostReplies.Where(p => p.PostId == post.Id).ToList();
            foreach (var item in postReplies)
            {
                _dbContext.PostReplies.Remove(item);
            }
            _dbContext.Posts.Remove(post);
            _dbContext.SaveChanges();
            _notifyService.Success(_localization.Getkey("DeletePostSuccess"));
            return RedirectToAction("Index", new { id = post.CategoryPostId });
        }
    }
}
