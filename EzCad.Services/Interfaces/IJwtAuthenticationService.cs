using Microsoft.AspNetCore.Identity;

namespace EzCad.Services.Interfaces;

public interface IJwtAuthenticationService<TUser> where TUser : IdentityUser
{
    string GetToken(TUser user);
}