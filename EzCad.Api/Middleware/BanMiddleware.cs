using EzCad.Services.Interfaces;
using EzCad.Shared.Responses;
using EzCad.Shared.Utils;

namespace EzCad.Api.Middleware;

public class BanMiddleware : IMiddleware
{
    private readonly IFrontendConfigurationService _frontendConfigurationService;

    public BanMiddleware(IFrontendConfigurationService frontendConfigurationService)
    {
        _frontendConfigurationService = frontendConfigurationService;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.User.IsInRole(RoleValues.Banned))
        {
            var configuration = await _frontendConfigurationService.GetConfigurationAsync();

            // They're banned, refuse
            var errorResponse = new BanResponse
            {
                Success = false,
                Message = $"You've been banned from {configuration.ServerName}"
            };

            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(errorResponse);

            return;
        }

        await next(context);
    }
}