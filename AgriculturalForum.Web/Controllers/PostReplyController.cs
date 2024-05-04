using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgriculturalForum.Web.Helper;
using AgriculturalForum.Web.Models;

namespace AgriculturalForum.Web.Controllers
{
    public class PostReplyController : Controller
    {

        private readonly KltnDbContext _dbContext;
        public PostReplyController(KltnDbContext dbContext)
        {
            _dbContext = dbContext;
        }
   
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(PostReply model)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");
            var account = _dbContext.Users.AsNoTracking().FirstOrDefault(u => u.Id == int.Parse(userId));
            if (account == null)
                return NotFound();
            if (model.Id == 0)
            {
                var newPostReply = new PostReply
                {
                    Id = model.Id,
                    Content = model.Content,
                    CreateDate = DateTime.Now,
                    PostId = model.PostId,
                    UserId = account.Id
                };

                _dbContext.PostReplies.Add(newPostReply);
                _dbContext.SaveChanges();

                return RedirectToAction("Detail", "Post", new { id = model.PostId });
            }
            else
            {
                var replyUpdate = _dbContext.PostReplies.FirstOrDefault(p => p.Id == model.Id);
                if (replyUpdate == null)
                    return NotFound();

                replyUpdate.Content = model.Content;
                _dbContext.PostReplies.Update(replyUpdate);
                _dbContext.SaveChanges();
                return RedirectToAction("Detail", "Post", new { id = replyUpdate.PostId });
            }
        }

        public IActionResult Delete(int id = 0)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");
            var postReply = _dbContext.PostReplies.Where(p => p.Id == id).FirstOrDefault();
            if (postReply == null)
                return NotFound();
            _dbContext.PostReplies.Remove(postReply);
            _dbContext.SaveChanges();
            return RedirectToAction("Detail", "Post", new { id = postReply.PostId });
        }
    }
}
