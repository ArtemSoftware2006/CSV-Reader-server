using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSV_Reader_server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TestController : ControllerBase
    {
        [Authorize]
        public Task<IActionResult> test()
        {
            return Task.FromResult<IActionResult>(Ok("Success"));
        }
    }
}