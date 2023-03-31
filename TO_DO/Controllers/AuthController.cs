using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TO_DO.DTOs.Auth;

namespace TO_DO.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<AuthTokenDto>> Login(
        [FromBody] LoginRequest request)
    {
        if(request is not { Login:"admin", Password: "admin" })
        {
            return Unauthorized();
        }
        var claims = new[]
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, "admin"),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, "admin")
        };
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
            Token = tokenValue
        };
    }
}
