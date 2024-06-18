using AgriculturalForum.Web.Interfaces;
using AgriculturalForum.Web.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace AgriculturalForum.Web.Controllers
{
    public class PostReplyController : Controller
    {

        private readonly IReplyRepository _replyRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotyfService _notyfService;
        public PostReplyController(IReplyRepository replyRepository, IUserRepository userRepository, INotyfService notyfService)
        {
            _replyRepository = replyRepository;
            _userRepository = userRepository;
            _notyfService = notyfService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(PostReply model)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var account = await _userRepository.GetUserById(int.Parse(userId));
            if (account == null)
                return NotFound();
            model.UserId = account.Id;

            if (model.Id == 0)
            {
                int id = await _replyRepository.Add(model);
                if (id == -1)
                {
                    _notyfService.Error("Bình luận không thành công");
                }
                return RedirectToAction("Detail", "Post", new { id = model.PostId });
            }
            else
            {
                bool result = await _replyRepository.Update(model);
                if (!result)
                {
                    _notyfService.Error("Chỉnh sửa bình luận không thành công");
                }
                return RedirectToAction("Detail", "Post", new { id = model.PostId });
            }
        }

        public async Task<IActionResult> Delete(int id = 0)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var postReply = await _replyRepository.GetById(id);
            if (postReply == null)
                return NotFound();

            await _replyRepository.Delete(id);
            return RedirectToAction("Detail", "Post", new { id = postReply.PostId });
        }
    }
}
