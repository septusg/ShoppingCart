using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string UserName { get; set; }

        [Required, StringLength(200)]
        public string Email { get; set; }

        [Required, StringLength(200)]
        public string PasswordHash { get; set; } // 暫存密碼 Hash
    }
}
