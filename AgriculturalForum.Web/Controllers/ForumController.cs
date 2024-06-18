using AgriculturalForum.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AgriculturalForum.Web.Controllers
{
    public class ForumController : Controller
    {
        private readonly IForumRepository _forumRepository;
        private readonly IPostRepository _postRepository;
        private readonly IReplyRepository _replyRepository;
        private readonly IUserRepository _userRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ForumController(IForumRepository forumRepository, IPostRepository postRepository, IReplyRepository replyRepository,
            IUserRepository userRepository, IWebHostEnvironment webHostEnvironment)
        {
            _forumRepository = forumRepository;
            _postRepository = postRepository;
            _replyRepository = replyRepository;
            _userRepository = userRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {

            var latestPostsByCategory = await _forumRepository.GetLatestPostOfCat();
            ViewBag.LatestPostOfCat = latestPostsByCategory;

            var recentPosts = await _postRepository.GetRecentPosts();
            ViewBag.RecentPosts = recentPosts;


            var totalPosts = await _postRepository.GetTotalPosts();
            var totalPostReplies = await _replyRepository.GetTotalReplies();
            var totalMembers = await _userRepository.GetTotalMembers();
            var latestMember = await _userRepository.GetLatestMember();

            ViewBag.TotalPosts = totalPosts;
            ViewBag.TotalPostReplies = totalPostReplies;
            ViewBag.TotalMembers = totalMembers;
            ViewBag.LatestMember = latestMember;

            var model = await _forumRepository.GetCatOfPosts();
            return View(model);
        }
    }
}
