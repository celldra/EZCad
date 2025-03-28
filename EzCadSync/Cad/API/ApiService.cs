using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using EzCadSync.Api.Exceptions;
using EzCadSync.Api.Forms;
using EzCadSync.Api.Models;
using EzCadSync.Api.Responses;
using EzCadSync.Api.Utils;
using Newtonsoft.Json;

namespace EzCadSync.Api;

public class ApiService : IDisposable
{
    private readonly HttpClient _client;
    private readonly List<IDisposable> _disposableObjects = new();

    public ApiService()
    {
        // Get configuration (either memory cached or fresh)
        var configuration = MemoryStorage.Configuration ?? ConfigurationLoader.LoadConfiguration();
        MemoryStorage.Configuration = configuration;

        _client = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
        });

        _client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("EZCadSync", "1.0.0"));
        _client.BaseAddress = new Uri(configuration.ApiBaseUrl);
    }

    public void Dispose()
    {
        DisposeObjects();
    }

    private async Task<HttpResponseMessage> PrepareAndSendAsync(HttpMethod method, string url, object? body = null)
    {
        var request = new HttpRequestMessage(method, url);

        _disposableObjects.Add(request);

        if (body is not null)
            request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

        var response = await _client.SendAsync(request);

        _disposableObjects.Add(response);

        return response;
    }

    private static T? PrepareResponse<T>(HttpResponseMessage message, string content)
    {
        if (message.StatusCode != HttpStatusCode.BadRequest)
        {
            if (typeof(T) == typeof(string)) return (T) (object) content;

            if (message.IsSuccessStatusCode) return JsonConvert.DeserializeObject<T>(content);

            var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(content);
            throw new ApiException(errorResponse?.Message ?? string.Empty, errorResponse);
        }

        var validationResponse = JsonConvert.DeserializeObject<ValidationErrorResponse>(content);
        throw new BadRequestException(validationResponse?.Message ?? string.Empty,
            validationResponse?.Errors ?? new List<ValidationError>());
    }

    public async Task<GameLoginResponse?> LoginAsync(string playerName, string licenseId)
    {
        try
        {
            var body = new GameLoginForm
            {
                Name = playerName,
                License = licenseId
            };

            var response = await PrepareAndSendAsync(HttpMethod.Post, "auth/game-login", body);
            var content = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.Unauthorized)
                return PrepareResponse<GameLoginResponse>(response, content);

            // They're banned, create an exception and throw
            var banResponse = JsonConvert.DeserializeObject<BanResponse>(content);

            throw new BannedException(banResponse.Message, banResponse);
        }
        finally
        {
            DisposeObjects();
        }
    }

    public async Task<IdentityResponse?> GetIdentityAsync(string licenseId)
    {
        try
        {
            var response = await PrepareAndSendAsync(HttpMethod.Get, $"user/{licenseId}");
            var content = await response.Content.ReadAsStringAsync();

            return PrepareResponse<IdentityResponse>(response, content);
        }
        finally
        {
            DisposeObjects();
        }
    }

    public async Task<string> GetVehicleAsync(string licenseId, string licensePlate)
    {
        try
        {
            var response =
                await PrepareAndSendAsync(HttpMethod.Get, $"vehicles/{licenseId}/license-plate/{licensePlate}");
            var content = await response.Content.ReadAsStringAsync();

            return PrepareResponse<string>(response, content);
        }
        finally
        {
            DisposeObjects();
        }
    }

    public async Task<TransactionResponse?> CollectSalaryAsync(string licenseId)
    {
        try
        {
            var response =
                await PrepareAndSendAsync(HttpMethod.Get, $"transactions/{licenseId}/collect-salary");
            var content = await response.Content.ReadAsStringAsync();

            return PrepareResponse<TransactionResponse>(response, content);
        }
        finally
        {
            DisposeObjects();
        }
    }

    public async Task<ErrorResponse?> LinkCadAsync(string licenseId, string id)
    {
        try
        {
            var response =
                await PrepareAndSendAsync(HttpMethod.Get, $"user/{id}/link/{licenseId}");
            var content = await response.Content.ReadAsStringAsync();

            return PrepareResponse<ErrorResponse>(response, content);
        }
        finally
        {
            DisposeObjects();
        }
    }

    public async Task<TransactionResponse?> FineAsync(string licenseId, string targetLicenseId, string description,
        double amount)
    {
        try
        {
            var body = new FineForm
            {
                Description = description,
                Amount = amount
            };

            var response =
                await PrepareAndSendAsync(HttpMethod.Post, $"identities/{licenseId}/fine/{targetLicenseId}", body);
            var content = await response.Content.ReadAsStringAsync();

            return PrepareResponse<TransactionResponse>(response, content);
        }
        finally
        {
            DisposeObjects();
        }
    }

    public async Task<BaseResponse?> UpgradeRankAsync(string licenseId, int rank)
    {
        try
        {
            var response = await PrepareAndSendAsync(HttpMethod.Get, $"transactions/{licenseId}/send-money");
            var content = await response.Content.ReadAsStringAsync();

            return PrepareResponse<ErrorResponse>(response, content);
        }
        finally
        {
            DisposeObjects();
        }
    }

    public async Task<TransactionResponse?> SendMoneyAsync(string licenseId, string targetLicenseId, string description,
        double amount)
    {
        try
        {
            var body = new SendMoneyForm
            {
                Description = description,
                ToLicenseId = targetLicenseId,
                Amount = amount
            };

            var response = await PrepareAndSendAsync(HttpMethod.Post, $"transactions/{licenseId}/send-money", body);
            var content = await response.Content.ReadAsStringAsync();

            return PrepareResponse<TransactionResponse>(response, content);
        }
        finally
        {
            DisposeObjects();
        }
    }

    public async Task<CreatedResponse<EmergencyReport>?> CreateEmergencyReportAsync(string licenseId, string description, string area,
        string postal)
    {
        try
        {
            var body = new EmergencyReportForm
            {
                Area = area,
                Description = description,
                PostCode = postal,
                ReporterLicenseId = licenseId
            };

            var response = await PrepareAndSendAsync(HttpMethod.Post, "emergency/reports", body);
            var content = await response.Content.ReadAsStringAsync();

            return PrepareResponse<CreatedResponse<EmergencyReport>>(response, content);
        }
        finally
        {
            DisposeObjects();
        }
    }

    public async Task CreateCriminalRecordAsync(string licenseId, string targetLicenseId, string action, string offence)
    {
        try
        {
            var body = new CriminalRecordForm
            {
                Action = action,
                Offence = offence,
                OffenderLicenseId = targetLicenseId,
                OfficerLicenseId = licenseId
            };

            var response = await PrepareAndSendAsync(HttpMethod.Post, "identities/criminal-records", body);
            var content = await response.Content.ReadAsStringAsync();

            _ = PrepareResponse<ErrorResponse>(response, content);
        }
        finally
        {
            DisposeObjects();
        }
    }

    private void DisposeObjects()
    {
        try
        {
            foreach (var obj in _disposableObjects) obj.Dispose();
        }
        catch (Exception)
        {
            // ignored.
        }
    }
}