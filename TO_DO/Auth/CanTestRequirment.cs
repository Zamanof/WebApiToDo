using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace TO_DO.Auth;

public class CanTestRequirment : 
    IAuthorizationRequirement, 
    IAuthorizationHandler
{
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        var claim = context.User.Claims.FirstOrDefault(c => c.Type == "permissions");
        if (claim != null)
        {
            var permissions = JsonSerializer.Deserialize<string[]>(claim.Value);
            if (permissions.Contains("CanTest"))
            {
                context.Succeed(this);
            }
        }
        else
        {
            context.Fail();
        }
        return Task.CompletedTask;
    }
}
