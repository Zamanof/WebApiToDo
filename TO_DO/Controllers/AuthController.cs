using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using TO_DO.Auth;
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
    private readonly IJwtService _jwtService;

    public AuthController(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IJwtService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _jwtService = jwtService;
    }


    [HttpPost("register")]
    public async Task<ActionResult<AuthTokenDto>> Register(
        [FromBody] RegisterRequest request)
    {
        var exisitingUser = await _userManager.FindByNameAsync(request.Email);
        if (exisitingUser is not null)
        {
            return Conflict("User already exists");
        }
        var user = new AppUser
        {
            UserName = request.Email,
            Email = request.Email,
            RefreshToken = Guid.NewGuid().ToString("N").ToLower()
        };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
        var role = await _userManager.GetRolesAsync(user);
        var userClaims = await _userManager.GetClaimsAsync(user);

        var accessToken = _jwtService.GenerateSecurityToken(user.Id, user.Email, role, userClaims);

        return new AuthTokenDto
        {
            AccessToken = accessToken,
            RefreshToken = user.RefreshToken
        };
    }


    [HttpPost("login")]
    public async Task<ActionResult<AuthTokenDto>> Login(
    [FromBody] LoginRequest request)
    {

        var user = await _userManager.FindByNameAsync(request.Email);
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

        var accessToken = _jwtService.GenerateSecurityToken(user.Id,user.Email, role, userClaims);

        user.RefreshToken= Guid.NewGuid().ToString("N").ToLower();
        await _userManager.UpdateAsync(user);
        return new AuthTokenDto
        {
            AccessToken = accessToken,
            RefreshToken = user.RefreshToken
        };
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthTokenDto>> Refresh([FromBody] RefreshTokenRequest request)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(e => e.RefreshToken == request.RefreshToken);
        
        if (user is null)
        {
            return Unauthorized();
        }
        var role = await _userManager.GetRolesAsync(user);
        var userClaims = await _userManager.GetClaimsAsync(user);

        var accessToken = _jwtService.GenerateSecurityToken(user.Id, user.Email, role, userClaims);

        user.RefreshToken = Guid.NewGuid().ToString("N").ToLower();
        await _userManager.UpdateAsync(user);
        return new AuthTokenDto
        {
            AccessToken = accessToken,
            RefreshToken = user.RefreshToken
        };
    }
}
