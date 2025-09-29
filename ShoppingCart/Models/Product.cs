using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingCart.Models
{
    public class Product
    {
        // 商品ID（主鍵）
        [Key]
        public int Id { get; set; }

        // 商品名稱
        [Required] // 必填
        [StringLength(100)] // 最大長度 100 字元
        public string Name { get; set; }

        // 商品描述
        public string Description { get; set; }

        // 價格
        [Range(0, double.MaxValue)] // 價格必須大於或等於 0
        public double Price { get; set; }

        // 商品數量（庫存）
        [Range(0, int.MaxValue)] // 數量大於等於 0
        public int StockQuantity { get; set; }

        // 商品類別（電子產品、衣物、食品等）
        [StringLength(50)] // 類別名稱最大長度 50 字元
        public string Category { get; set; }
    }
}