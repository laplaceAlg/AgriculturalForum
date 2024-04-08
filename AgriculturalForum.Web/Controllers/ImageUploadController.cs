using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace AgriculturalForum.Web.Controllers
{
    public class ImageUploadController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ImageUploadController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpPost]
        public async Task<JsonResult> UploadFile(IFormFile file)
        {
            var returnImagePath = string.Empty;
            if (file != null && file.Length > 0)
            {
                var extension = Path.GetExtension(file.FileName);

                string imageName =  DateTime.Now.Ticks.ToString();
                var imageSavePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", imageName + extension);

                returnImagePath = "/images/" + imageName + extension;

                
                using (var stream = new FileStream(imageSavePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                
                TempData["message"] = "Image was added successfully";
            }
            else
            {
                TempData["message"] = "Invalid file";
            }

            return Json(returnImagePath);
        }
    }
}
