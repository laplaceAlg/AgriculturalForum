using AgriculturalForum.Web.Interfaces;
using AgriculturalForum.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace AgriculturalForum.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly KltnDbContext _dbcontext;
        private readonly IUserRepository _userRepository;
        private readonly IPostRepository _postRepository;
        private readonly IProductRepository _productRepository;
        public UserController(IUserRepository userRepository, IPostRepository postRepository,
            IProductRepository productRepository, KltnDbContext context)
        {
            _userRepository = userRepository;
            _postRepository = postRepository;
            _productRepository = productRepository;
            _dbcontext = context;
        }

        public async Task<IActionResult> Index(int id = 0)
        {
            var model = await _userRepository.GetUserById(id);

            var lsPosts = await _postRepository.GetPostsByUserId(id);
            var modelPosts = lsPosts.Take(4).ToList();
            ViewBag.ListPosts = modelPosts;

            var lsProducts = await _productRepository.GetProductsByUserId(id);
            var modelProducts = lsProducts.Take(4).ToList();
            ViewBag.ListProducts = modelProducts;
            var totalPosts = await _userRepository.GetTotalPosts(id);
            ViewBag.TotalPost = totalPosts;
            var totalProducts = await _userRepository.GetTotalProducts(id);
            ViewBag.TotalProduct = totalProducts;
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }
    }
}
