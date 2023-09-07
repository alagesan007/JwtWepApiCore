using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JwtWepApiCore.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        public IEnumerable<string> get()
        {
            return new[] { "alagesh", "saravan", "thaunggs" };
        }
    }
}
