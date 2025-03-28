using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Blazored.LocalStorage;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using EzCad.Services;
using EzCad.Services.Interfaces;
using EzCad.Web.Providers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

#if RELEASE
builder.WebHost.UseUrls("http://localhost:5000");
#endif

// Add services to the container.
builder.Services
    .AddBlazorise(options => { options.Immediate = true; })
    .AddBootstrap5Providers()
    .AddFontAwesomeIcons();
builder.Services.AddLogging(options =>
{
#if DEBUG
    if (OperatingSystem.IsLinux())
    {
        options.AddSystemdConsole();
        options.SetMinimumLevel(LogLevel.Information);
    }
    else
    {
        options.AddConsole();
        options.SetMinimumLevel(LogLevel.Information);
    }
#else
     options.AddConsole();
        options.SetMinimumLevel(LogLevel.Information);
#endif
});
builder.Services.AddSingleton<IBackendConfigurationService, BackendConfigurationService>();

var services = builder.Services.BuildServiceProvider();

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.MimeTypes = new[]
    {
        "text/html",
        "text/css",
        "text/javascript",
        "application/javascript",
        "image/png",
        "image/webp",
        "text/plain"
    };
    options.Providers.Add(new BrotliCompressionProvider(new BrotliCompressionProviderOptions()));
    options.Providers.Add(new GzipCompressionProvider(new GzipCompressionProviderOptions()));
});
builder.Services.AddHttpClient<HttpClient>("api", options =>
{
    options.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("EZCad-Frontend",
        Assembly.GetEntryAssembly()?.GetName().Version?.ToString()));
    options.BaseAddress = new Uri(services.GetRequiredService<IBackendConfigurationService>().Configuration.ApiBaseUrl);
});
builder.Services.Configure<JsonSerializerOptions>(options =>
{
    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.WriteIndented = true;
    options.AllowTrailingCommas = true;
    options.IncludeFields = true;
    options.PropertyNameCaseInsensitive = true;
    options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddResponseCaching();
builder.Services.AddHostFiltering(options =>
{
    options.AllowEmptyHosts = false;
    options.IncludeFailureMessage = false;
    options.AllowedHosts = services.GetRequiredService<IBackendConfigurationService>().Configuration.Domains;
});

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.RequestPath | HttpLoggingFields.RequestMethod |
                            HttpLoggingFields.ResponseStatusCode | HttpLoggingFields.RequestProtocol;
});

builder.Services.AddAuthorizationCore();
builder.Services.AddAuthenticationCore();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthStateProvider>();
builder.Services.AddScoped<IClientJavascriptService, ClientJavascriptService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHostFiltering();

#if RELEASE
app.UseHttpsRedirection();
#endif

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();

app.UseResponseCompression();
app.UseResponseCaching();

app.MapBlazorHub();
app.MapRazorPages();
app.MapFallbackToPage("/_Host");

app.Run();