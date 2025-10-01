using ShoppingCart.Models;


namespace ShoppingCart.Data
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext db)
        {
            // 先確保資料庫已存在
            db.Database.EnsureCreated();

            // 新增預設使用者（如果不存在才新增）
            if (!db.Users.Any(u => u.Email == "abc@gmail.com"))
            {
                db.Users.Add(new User
                {
                    UserName = "abc",
                    Email = "abc@gmail.com",
                    PasswordHash = ComputeHash("abc123")
                });
            }

            // 新增商品（如果不存在才新增）
            if (!db.Products.Any())
            {
                db.Products.AddRange(
                    new Product { Name = "貓貓耳機", Price = 1299, StockQuantity = 10, Category = "電子產品" }
                );
            }

            db.SaveChanges();
        }

        private static string ComputeHash(string input)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(sha256.ComputeHash(bytes));
        }
    }
}