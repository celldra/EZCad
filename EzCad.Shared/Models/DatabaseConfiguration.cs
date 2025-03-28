using System.Text;
using System.Text.Json.Serialization;

namespace EzCad.Shared.Models;

public class DatabaseConfiguration
{
    [JsonPropertyName("isEnabled")] public bool IsEnabled { get; set; }

    [JsonPropertyName("host")] public string Host { get; set; } = "127.0.0.1";

    [JsonPropertyName("port")] public ushort? Port { get; set; } = null;

    [JsonPropertyName("username")] public string? Username { get; set; } = "REPLACE_WITH_USERNAME";

    [JsonPropertyName("password")] public string? Password { get; set; } = "REPLACE_WITH_PASSWORD";

    [JsonPropertyName("name")] public string? Name { get; set; } = "EZCad";

    public string BuildRedisConnectionString()
    {
        var builder = new StringBuilder();
        builder.Append($"{Host}:");

        if (Port is not null) builder.Append(Port);
        else builder.Append(6379);

        return builder.ToString();
    }

    public string BuildPostgresConnectionString()
    {
        var builder = new StringBuilder();
        builder.Append($"Host={Host};");

        if (Port is not null) builder.Append($"Port={Port};");
        else builder.Append("Port=5432;");

        if (Name is not null) builder.Append($"Database={Name};");
        else builder.Append("Database=EZCad;");

        if (Username is not null) builder.Append($"Username={Username};");
        if (Password is not null) builder.Append($"Password={Password};");

#if DEBUG
        builder.Append("Include Error Detail=true;");
#endif

        return builder.ToString();
    }
}