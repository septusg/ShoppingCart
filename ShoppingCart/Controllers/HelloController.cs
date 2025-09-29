using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ShoppingCart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HelloController : ControllerBase
    {
        // 最基本的 GET 接口
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("測試用：第一支 API");
        }
    }
}