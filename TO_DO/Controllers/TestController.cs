using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TO_DO.Models;

namespace TO_DO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [Authorize(Policy ="CanTest")]
        [Authorize(Policy ="CanDelete")]
        [HttpGet("test")]
        public async Task<ActionResult> Test()
        {
            return Ok("It's works");
        }
    }
}
