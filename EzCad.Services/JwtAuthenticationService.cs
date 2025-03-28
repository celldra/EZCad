using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EzCad.Database;
using EzCad.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace EzCad.Services;

public class JwtAuthenticationService<TUser> : IJwtAuthenticationService<TUser> where TUser : IdentityUser
{
    private readonly IBackendConfigurationService _backendConfiguration;
    private readonly EzCadDataContext _context;
    private readonly JwtSecurityTokenHandler _handler = new();
    private readonly ILogger<JwtAuthenticationService<TUser>> _logger;

    public JwtAuthenticationService(EzCadDataContext context, IBackendConfigurationService backendConfiguration,
        ILogger<JwtAuthenticationService<TUser>> logger)
    {
        _context = context;
        _backendConfiguration = backendConfiguration;
        _logger = logger;
    }

    public string GetToken(TUser user)
    {
        var roles = _context.UserRoles
            .Join(_context.Roles, userRoles => userRoles.RoleId, role => role.Id,
                (userRoles, role) => new {userRoles, role})
            .Where(t => t.userRoles.UserId == user.Id)
            .Select(t => new {t.userRoles.UserId, t.userRoles.RoleId, t.role.Name});

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email),
            new(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString()),
            new(JwtRegisteredClaimNames.Exp,
                new DateTimeOffset(DateTime.UtcNow.AddHours(12)).ToUnixTimeSeconds().ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Aud, "EZCad"),
            new(JwtRegisteredClaimNames.Iss, _backendConfiguration.Configuration.JwtIssuer),
            new(JwtRegisteredClaimNames.Website,
                _backendConfiguration.Configuration.Domains.FirstOrDefault() ?? "https://ezcad.io/"),
            new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString())
        };

        foreach (var role in roles) claims.Add(new Claim(ClaimTypes.Role, role.Name));

        return ConstructToken(claims);
    }

    private string ConstructToken(IEnumerable<Claim> claims)
    {
        var key = Encoding.UTF8.GetBytes(_backendConfiguration.Configuration.JwtSigningKey);

        var token = new JwtSecurityToken(
            new JwtHeader(new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)),
            new JwtPayload(claims));

        _logger.LogInformation("Wrote and signed new ambiguous JWT token");

        return _handler.WriteToken(token);
    }
}