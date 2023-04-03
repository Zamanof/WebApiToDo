using Microsoft.AspNetCore.Identity;

namespace TO_DO.Models;
public class AppUser: IdentityUser
{
    public string? RefreshToken { get; set; }
}
