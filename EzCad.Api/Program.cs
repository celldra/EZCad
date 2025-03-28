using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using EzCad.Api.Exceptions;
using EzCad.Api.Hashers;
using EzCad.Api.Middleware;
using EzCad.Database;
using EzCad.Database.Entities;
using EzCad.Extensions.Discord;
using EzCad.Redis;
using EzCad.Services;
using EzCad.Services.Interfaces;
using EzCad.Shared.Models;
using EzCad.Shared.Responses;
using EzCad.Shared.Utils;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;

const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

var builder = WebApplication.CreateBuilder(args);

#if RELEASE
builder.WebHost.UseUrls("http://localhost:5001");
#endif

#region Inject mapping objects

builder.Services.AddAutoMapper(options =>
{
    options.CreateMap<BanRecord, BanResponse>()
        .ForMember(m => m.Message, a => a.Ignore())
        .ForMember(m => m.Success, a => a.Ignore())
        .ForMember(m => m.DateBanned, a => a.MapFrom(m => m.DateCreated))
        .ForMember(m => m.Reason, a => a.MapFrom(m => m.Reason))
        .ForMember(m => m.Expiration, a => a.MapFrom(m => m.Expiration))
        .ForMember(m => m.IsPermanent, a => a.MapFrom(m => m.IsPermanent))
        .ForMember(m => m.BannedBy, a => a.MapFrom(m => m.BannedBy.UserName));

    options.CreateMap<User, UserProfile>()
        .ForMember(m => m.Roles, a => a.Ignore())
        .ForMember(m => m.Message, a => a.Ignore())
        .ForMember(m => m.Success, a => a.Ignore())
        .ForMember(m => m.DateCreated, a => a.MapFrom(m => m.DateCreated))
        .ForMember(m => m.Email, a => a.MapFrom(m => m.Email))
        .ForMember(m => m.UserName, a => a.MapFrom(m => m.UserName))
        .ForMember(m => m.IsLinked, a => a.MapFrom(m => m.IsLinked))
        .ForMember(m => m.License, a => a.MapFrom(m => m.LicenseId))
        .ForMember(m => m.Id, a => a.MapFrom(m => m.Id))
        .ForMember(m => m.BanRecords, a => a.MapFrom(m => m.BanRecords))
        .ForMember(m => m.DiscordId, a => a.MapFrom(m => m.DiscordId));

    options.CreateMap<IdentityRole, UserRole>()
        .ForMember(m => m.Id, a => a.MapFrom(m => m.Id))
        .ForMember(m => m.Name, a => a.MapFrom(m => m.Name))
        .ForMember(m => m.IsDefault, a => a.MapFrom(m => RoleValues.GetAllDefaultRoles().Contains(m.Name)));

    options.CreateMap<Configuration, FrontendConfiguration>()
        .ForMember(m => m.Currency, a => a.MapFrom(m => m.Currency))
        .ForMember(m => m.ConnectUrl, a => a.MapFrom(m => m.ConnectUrl))
        .ForMember(m => m.PrimaryHexColor, a => a.MapFrom(m => m.PrimaryHexColor))
        .ForMember(m => m.ServerName, a => a.MapFrom(m => m.ServerName))
        .ForMember(m => m.Id, a => a.MapFrom(m => m.Id))
        .ForMember(m => m.DateCreated, a => a.MapFrom(m => m.DateCreated));
});

#endregion


#region Inject logging

// Add services to the container.
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

#endregion


#region Inject database and configuration services

// Temporarily create a backend configuration service so we can access the DB options without building the provider
IBackendConfigurationService backendConfiguration =
    new BackendConfigurationService(new NullLogger<BackendConfigurationService>());

builder.Services.AddSingleton<IBackendConfigurationService, BackendConfigurationService>();
builder.Services.AddScoped<IFrontendConfigurationService, FrontendConfigurationService>();

builder.Services.AddDbContext<EzCadDataContext>(config => config
        .UseNpgsql(backendConfiguration.Configuration.DatabaseConfiguration
            .BuildPostgresConnectionString())
        .UseLazyLoadingProxies()
        .EnableServiceProviderCaching()
#if DEBUG
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()
#endif
);

#endregion


#region Inject authentication services and security services

// Add Argon2id hasher
builder.Services.AddTransient<IPasswordHasher<User>, ArgonHasher>();

// Add default identity
builder.Services.AddIdentity<User, IdentityRole>(config =>
    {
        config.Lockout.AllowedForNewUsers = true;
        config.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        config.Lockout.MaxFailedAccessAttempts = 5;
        config.Password.RequireDigit = true;
        config.Password.RequireLowercase = true;
        config.Password.RequiredLength = 6;
        config.Password.RequireNonAlphanumeric = false;
        config.Password.RequireUppercase = true;
        config.User.RequireUniqueEmail = true;
        config.User.AllowedUserNameCharacters = allowedChars;
        config.SignIn.RequireConfirmedEmail = false;
    })
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<EzCadDataContext>();


// Add authentication
builder.Services.AddAuthorization()
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "JwtBearer";
        options.DefaultChallengeScheme = "JwtBearer";
    })
    .AddJwtBearer("JwtBearer", jwtBearerOptions =>
    {
        jwtBearerOptions.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(backendConfiguration.Configuration.JwtSigningKey)),
                ValidateAudience = true,
                RequireAudience = true,
                RequireExpirationTime = true,
                ValidAudiences = new[] {"EZCad"},
                ValidateIssuer = true,
                ValidIssuer = backendConfiguration.Configuration.JwtIssuer,
                RequireSignedTokens = true,
                ValidateLifetime = true,
                ValidAlgorithms = new[] {"HS256"},
                ValidateTokenReplay = true,
                ClockSkew = TimeSpan.FromMinutes(5)
            };
    });

#endregion


#region Add caching services (including Redis)

builder.Services.AddRedisCaching(options =>
{
    options.Host = backendConfiguration.Configuration.RedisConfiguration.Host;
    options.IsEnabled = backendConfiguration.Configuration.RedisConfiguration.IsEnabled;
    options.Port = backendConfiguration.Configuration.RedisConfiguration.Port;
    options.Name = backendConfiguration.Configuration.RedisConfiguration.Name;
});
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
builder.Services.AddResponseCaching();
builder.Services.AddMemoryCache();

#endregion


#region Miscellaneous services

builder.Services.AddHttpClient<HttpClient>("api", options =>
{
    options.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("EzCad-API",
        Assembly.GetEntryAssembly()?.GetName().Version?.ToString()));
});

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.RequestPath | HttpLoggingFields.RequestMethod |
                            HttpLoggingFields.ResponseStatusCode | HttpLoggingFields.RequestProtocol;
});

#endregion


#region Application services

// Add middleware
builder.Services.AddScoped<BanMiddleware>();

// Add services
builder.Services.AddScoped<IJwtAuthenticationService<User>, JwtAuthenticationService<User>>();
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<ICriminalService, CriminalService>();
builder.Services.AddScoped<IGameLoginService, GameLoginService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmergencyService, EmergencyService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddDiscord(options =>
{
    options.Secret = backendConfiguration.Configuration.DiscordConfiguration.Secret;
    options.ServerId = backendConfiguration.Configuration.DiscordConfiguration.ServerId;
    options.ShouldAutoJoinServer = backendConfiguration.Configuration.DiscordConfiguration.ShouldAutoJoinServer;
    options.RedirectDomain = backendConfiguration.Configuration.DiscordConfiguration.RedirectDomain;
    options.ClientId = backendConfiguration.Configuration.DiscordConfiguration.ClientId;
    options.BotToken = backendConfiguration.Configuration.DiscordConfiguration.BotToken;
    options.IsEnabled = backendConfiguration.Configuration.DiscordConfiguration.IsEnabled;
});

#endregion


#region ASP.NET Web API services

// Add other ASP.NET stuff
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.AllowTrailingCommas = true;
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.AllowInputFormatterExceptionMessages = true;
    });

#if DEBUG
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endif

builder.Services.AddRouting();

builder.Services.AddHostFiltering(options =>
{
    options.AllowEmptyHosts = false;
    options.IncludeFailureMessage = false;
    options.AllowedHosts = backendConfiguration.Configuration?.Domains ?? Array.Empty<string>();
});

#endregion


#region Configure already injected services

builder.Services.Configure<JsonSerializerOptions>(options =>
{
    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.WriteIndented = true;
    options.AllowTrailingCommas = true;
    options.IncludeFields = true;
    options.PropertyNameCaseInsensitive = true;
    options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.Configure<ApiBehaviorOptions>(options => options.InvalidModelStateResponseFactory = context =>
{
    var problemDetails = new ValidationProblemDetails(context.ModelState);
    throw new ValidationException(problemDetails.Errors);
});

#endregion

var app = builder.Build();

app.UseExceptionHandler(errorApp => errorApp.UseMiddleware<ErrorHandlingMiddleware>());

// Configure the HTTP request pipeline.
#if DEBUG
app.UseSwagger();
app.UseSwaggerUI();
#else
app.UseHttpsRedirection();
#endif

app.UseRouting();
app.UseHostFiltering();

app.UseAuthentication();
app.UseAuthorization();
app.UseResponseCaching();
app.UseResponseCompression();

// Use ban detection middleware
app.UseMiddleware<BanMiddleware>();

app.MapControllers();

app.Run();