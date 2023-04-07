using System.Security.Claims;

namespace TO_DO.Auth;

public interface IJwtService
{
    string GenerateSecurityToken(
        string email,
        IEnumerable<string> roles,
        IEnumerable<Claim> userClaims);
}
