﻿using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace TO_DO.Auth;

public class JwtService : IJwtService
{
    private readonly JwtConfig _config;

    public JwtService(JwtConfig config)
    {
        _config = config;
    }

    public string GenerateSecurityToken(
        string id,
        string email, 
        IEnumerable<string> roles, 
        IEnumerable<Claim> userClaims)
    {
        var claims = new[]
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, email),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, string.Join(",", roles)),
            new Claim("userId", id),
            new Claim("permissions",JsonSerializer.Serialize( new String[]{"CanTest", "CanRead"}))
        }.Concat(userClaims);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Secret));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config.Issuer,
            audience: _config.Auidience,
            expires: DateTime.UtcNow.AddMinutes(_config.ExpiresInMinutes),
            signingCredentials: signingCredentials,
            claims: claims
            );
        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        return accessToken;
    }
}
