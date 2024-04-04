using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ModelViews
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = ("Vui lòng nhập Email"))]
        [EmailAddress(ErrorMessage = "Sai định dạng Email")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string Password { get; set; }
    }
}
