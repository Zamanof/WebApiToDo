using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using TO_DO.DTOs.Auth;
using TO_DO.Models;

namespace TO_DO.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AuthController(
        UserManager<AppUser> userManager, 
        SignInManager<AppUser> signInManager, 
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }
  
    [HttpPost("login")]
    public async Task<ActionResult<AuthTokenDto>> Login(
        [FromBody] LoginRequest request)
    {

        var user = await _userManager.FindByNameAsync(request.Login);
        if (user is null)
        {
            return Unauthorized();
        }
        var canSignIn = await _signInManager
            .CheckPasswordSignInAsync(user, request.Password, false);
        if (!canSignIn.Succeeded)
        {
            return Unauthorized();
        }
        var role = await _userManager.GetRolesAsync(user);
        var userClaims = await _userManager.GetClaimsAsync(user);

        var claims = new[]
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, string.Join(",", role))
            //new Claim("CanTest", "true")
            //new Claim("permissions", JsonSerializer.Serialize(
            //    new []
            //    {
            //        "CanTest", 
            //        "CanDelete",

            //    }))
        }.Concat(userClaims);

        // AAD, B2C, Cognito, KeyCloack

        // admin
        // moderator
        // guest
        // user
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Super Sequrity Key"));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "https://localhost:5000",
            audience: "https://localhost:5000",            
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: signingCredentials,
            claims: claims
            );
        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
        return new AuthTokenDto
        {
            AccessToken = tokenValue
        };
    }
}
