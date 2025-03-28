using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace EzCad.Web;

public static class ExtensionMethods
{
    public static void SetAuthorizationHeader(this HttpClient client, string t)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", t);
    }

    public static async Task<T?> GetJsonAsync<T>(this HttpClient client, string url,
        CancellationToken cancellationToken = default)
    {
        var response = await client.GetAsync(url, cancellationToken);
        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        
        if (typeof(T) == typeof(bool))
        {
            return (T)(object)response.IsSuccessStatusCode;
        }
        
        if (response.StatusCode != HttpStatusCode.NoContent)
        {
            return await JsonSerializer.DeserializeAsync<T>(stream, cancellationToken: cancellationToken);
        }

        return default;
    }

    public static async Task<T?> DeleteJsonAsync<T>(this HttpClient client, string url,
        CancellationToken cancellationToken = default)
    {
        var response = await client.DeleteAsync(url, cancellationToken);
        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        if (typeof(T) == typeof(bool))
        {
            return (T)(object)response.IsSuccessStatusCode;
        }
        
        if (response.StatusCode != HttpStatusCode.NoContent)
        {
            return await JsonSerializer.DeserializeAsync<T>(stream, cancellationToken: cancellationToken);
        }

        return default;
    }

    public static async Task<T?> PostJsonAsync<T>(this HttpClient client, string url, object? content = null,
        CancellationToken cancellationToken = default)
    {
        var stringContent = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");
        var response = await client.PostAsync(url, stringContent, cancellationToken);
        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        return await JsonSerializer.DeserializeAsync<T>(stream, cancellationToken: cancellationToken);
    }

    public static async Task<T?> PutJsonAsync<T>(this HttpClient client, string url, object? content = null,
        CancellationToken cancellationToken = default)
    {
        var stringContent = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");
        var response = await client.PutAsync(url, stringContent, cancellationToken);
        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        if (typeof(T) == typeof(bool))
        {
            return (T)(object)response.IsSuccessStatusCode;
        }
        
        if (response.StatusCode != HttpStatusCode.NoContent)
        {
            return await JsonSerializer.DeserializeAsync<T>(stream, cancellationToken: cancellationToken);
        }

        return default;
    }
}