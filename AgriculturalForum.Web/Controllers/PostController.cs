using AgriculturalForum.Web.Extensions;
using AgriculturalForum.Web.Interfaces;
using AgriculturalForum.Web.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using PagedList.Core;


namespace AgriculturalForum.Web.Controllers
{
    public class PostController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly ICategoryPostRepository _categoryPostRepository;
        private readonly IUserRepository _userRepository;
        private LanguageService _localization;
        private readonly INotyfService _notifyService;
        const string CREATE_TITLE = "NewPost";
        const string EDIT_TITLE = "UpdatePost";
        public PostController(IPostRepository postRepository, IUserRepository userRepository,
            ICategoryPostRepository categoryPostRepository, LanguageService localization, INotyfService notifyService)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _categoryPostRepository = categoryPostRepository;
            _localization = localization;
            _notifyService = notifyService;
        }

        public async Task<IActionResult> Index(int page = 1, int id = 0)
        {
            var category = await _categoryPostRepository.GetById(id);
            if (category != null)
                ViewBag.Title = category.Title;

            var pageNumber = page;
            var pageSize = 3;

            var lsPosts = await _postRepository.GetPostsByCategory(id);

            PagedList<Post> models = new PagedList<Post>(lsPosts.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentCateID = id;
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        public async Task<IActionResult> Detail(int id = 0)
        {
            var post = await _postRepository.GetById(id);
            if (post == null)
                return NotFound();

            var similarPosts = await _postRepository.GetSimilarPosts(post.CategoryPostId, id);

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

        public async Task<IActionResult> Edit(int id = 0)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account", new { ReturnUrl = "/Post/Edit" });

            ViewBag.Title = _localization.Getkey(EDIT_TITLE);
            ViewBag.IsEdit = true;
            var model = await _postRepository.GetById(id);
            if (model == null)
                return RedirectToAction("Index");

            if (string.IsNullOrWhiteSpace(model.Image))
                model.Image = "default.jpg";

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(Post model, IFormFile? imgFile = null)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");
            var account = await _userRepository.GetUserById(int.Parse(userId));
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
                model.UserId = account.Id;
                int id = await _postRepository.Add(model, imgFile);
                if (id == -1)
                {
                    ViewBag.Title = _localization.Getkey(CREATE_TITLE);
                    return View("Edit", model);
                }
                _notifyService.Success(_localization.Getkey("CreatePostSuccess"));
                return RedirectToAction("Index", new { id = model.CategoryPostId });

            }
            else
            {
                bool result = await _postRepository.Update(model, imgFile);
                if (!result)
                {
                    ViewBag.Title = _localization.Getkey(EDIT_TITLE);
                    return View("Edit", model);
                }
                _notifyService.Success(_localization.Getkey("UpdatePostSuccess"));
                return RedirectToAction("Detail", new { id = model.Id });
            }
            /* return RedirectToAction("Edit");*/
        }

        public async Task<IActionResult> Delete(int id = 0)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");
            var post = await _postRepository.GetById(id);
            if (post == null)
                return NotFound();
            await _postRepository.Delete(id);
            _notifyService.Success(_localization.Getkey("DeletePostSuccess"));
            return RedirectToAction("Index", new { id = post.CategoryPostId });
        }
    }
}
