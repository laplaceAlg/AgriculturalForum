using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AgriculturalForum.Web.ModelViews
{
    public class ChangePasswordViewModel
    {
        [Key]
        public int UserId { get; set; }

        [DisplayName("OldPassword")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại")]
        public string PasswordNow { get; set; }

        [DisplayName("NewPassword")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [MinLength(5, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 5 ký tự")]
        public string Password { get; set; }

        [MinLength(5, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 5 ký tự")]
        [DisplayName("ConfirmPassword")]
        [Compare("Password", ErrorMessage = "Nhập lại mật khẩu không đúng")]
        public string ConfirmPassword { get; set; }
    }
}
