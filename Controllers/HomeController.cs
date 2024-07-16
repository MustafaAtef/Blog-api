using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers {

    [ApiController]
    [Route("[Controller]")]
    public class HomeController : ControllerBase{
        [HttpGet]
        [Authorize]
        public string Get() {
            var user = HttpContext.User;
            return $"Authenticated!!";
        }
    }
}
