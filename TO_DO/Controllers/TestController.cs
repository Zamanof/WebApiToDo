using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TO_DO.Models;

namespace TO_DO.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public TestController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [Authorize(Policy = "CanTest")]
        [Authorize(Policy = "CanDelete")]
        [HttpGet("test")]
        public async Task<ActionResult> Test()
        {
            return Ok("It's works");
        }
        [HttpPost("Add role")]
        public async Task<ActionResult> AddRole(string roleName)
        {
            if (!_roleManager.RoleExistsAsync(roleName).Result)
            {
                var role = new IdentityRole(roleName);
                await _roleManager.CreateAsync(role);
            }
            return Ok();
        }
    }


}
