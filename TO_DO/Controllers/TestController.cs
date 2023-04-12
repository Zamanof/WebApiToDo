using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using TO_DO.Models;


namespace TO_DO.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger _logger;
        private readonly IMemoryCache _memoryCache;

        public TestController(RoleManager<IdentityRole> roleManager, ILogger<TestController> logger, IMemoryCache memoryCache)
        {
            _roleManager = roleManager;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        //[Authorize(Policy = "CanTest")]
        //[Authorize(Policy = "CanDelete")]
        //[ResponseCache(Duration = 30)]
        [HttpGet("test")]
        public async Task<ActionResult> Test()
        {

            if (_memoryCache.TryGetValue("test", out var memory))
            {
                return Ok(memory);
            }
            else
            {
                await Task.Delay(3000);
                _memoryCache.Set("test", "It's works -> 200", new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromSeconds(3)
                }) ;
                return Ok("It's works -> 200");
            }
            //await Task.Delay(3000);
            ////_logger.LogInformation("It's works -> 200");
            //_logger.LogError("It's works -> 200");
            ////Log.Information("It's works -> 200");
            ////Log.Error(new ArgumentException("It's works -> 200", "atribute"), "");
            //return Ok("It's works");
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
