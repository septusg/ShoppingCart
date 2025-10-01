using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Models.Dto
{
    public class RegisterDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // 前端送明文密碼，後端在收到後 Hash 並存在 User.PasswordHash
        [Required]
        public string Password { get; set; }
    }

    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
