using System.Net.Http.Headers;
using EzCad.Extensions.Discord.Interfaces;
using EzCad.Extensions.Discord.Models;
using Microsoft.Extensions.DependencyInjection;

namespace EzCad.Extensions.Discord;

/// <summary>
///     Contains static miscellaneous extension methods to perform other tasks not specifically tied down to a single class
/// </summary>
public static class ExtensionMethods
{
    public static IServiceCollection AddDiscord(this IServiceCollection services, Action<DiscordConfiguration> options)
    {
        // Throw an exception if the configuration is not set
        ArgumentNullException.ThrowIfNull(options);

        // Convert the action into an accessible model
        var configuration = new DiscordConfiguration();
        options(configuration);

        // Add and configure the HttpClientFactory for the service
        services.AddHttpClient<HttpClient>("discordapi", client =>
        {
            client.BaseAddress = new Uri("https://discord.com/api/");
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("EZCad-Discord",
                typeof(DiscordService).Assembly.GetName().Version?.ToString()));
        });

        // Now add the main service
        return services.AddSingleton<IDiscordService>(s =>
            new DiscordService(s.GetRequiredService<IHttpClientFactory>(), configuration));
    }
}