using Microsoft.EntityFrameworkCore;
using ShoppingCart.Models;


namespace ShoppingCart.Data
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext db)
        {
            Console.WriteLine($"Seeding DB -> {db.Database.GetDbConnection().ConnectionString}");

            // 新增使用者（不存在才新增）
            if (!db.Users.Any(u => u.Email == "abc@gmail.com"))
            {
                db.Users.Add(new User
                {
                    UserName = "abc",
                    Email = "abc@gmail.com",
                    PasswordHash = ComputeHash("abc123")
                });
            }

            // 新增商品（不存在才新增）
            if (!db.Products.Any())
            {
                db.Products.AddRange(
                    new Product { Name = "貓貓耳機", Description = "可愛又有RGB燈光！", Price = 1299, StockQuantity = 10, Category = "電子產品", ImageUrl = "https://i1.momoshop.com.tw/1721715306/goodsimg/0012/134/456/12134456_O_m.webp" },
                    new Product { Name = "狗狗滑鼠", Description = "可愛滑鼠", Price = 599, StockQuantity = 15, Category = "電子產品", ImageUrl = "https://m.media-amazon.com/images/I/51vtWIoKyzL._UF1000,1000_QL80_.jpg" }
                );
            }

            db.SaveChanges();
            Console.WriteLine("Seeding finished.");
        }


        private static string ComputeHash(string input)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(sha256.ComputeHash(bytes));
        }
    }
}