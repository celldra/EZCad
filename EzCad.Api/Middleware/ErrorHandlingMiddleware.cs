using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using EzCad.Api.Exceptions;
using EzCad.Api.Models;
using EzCad.Api.Responses;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Net.Http.Headers;

namespace EzCad.Api.Middleware;

public class ErrorHandlingMiddleware : BaseMiddleware
{
    public async Task Invoke(HttpContext context, ILogger<ErrorHandlingMiddleware> logger)
    {
        var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
        var exception = errorFeature?.Error;

        var problemDetails = new ValidationErrorResponse
        {
            Success = false,
            Message = "An unhandled API error has occurred"
        };

        switch (exception)
        {
            case ValidationException validationException:
                context.Response.StatusCode = 400;
                
                problemDetails.Message = "One or more validation errors have occurred";
                problemDetails.Errors = validationException.Errors.Select(x => new ValidationError
                {
                    Field = x.Key,
                    Message = x.Value.Aggregate(string.Empty, (c, s) => c + $"{s}, ").TrimEnd(' ').TrimEnd(',')
                }).ToList();
                break;
            default:
                context.Response.StatusCode = 500;
                
                if (exception != null)
                    logger.LogError("Exception: {ExceptionObject}", exception);

                if (!Directory.Exists("errors")) Directory.CreateDirectory("errors");
                var errorLogFile =
                    $"error-log-{DateTimeOffset.Now.ToUnixTimeMilliseconds()}-{Activity.Current?.Id ?? context.TraceIdentifier}.txt";
                await File.WriteAllTextAsync(Path.Combine("errors", errorLogFile), exception?.ToString());
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
        {
            NoCache = true
        };
        
        await JsonSerializer.SerializeAsync(context.Response.Body, problemDetails,
            new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNameCaseInsensitive = true
            });
    }

    public ErrorHandlingMiddleware(RequestDelegate request) : base(request)
    {
    }
}