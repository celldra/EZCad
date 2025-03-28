using System.Text.Json.Serialization;

namespace EzCad.Database.Entities;

public class UserRole
{
    [JsonPropertyName("id")] public string Id { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("isDefault")] public bool IsDefault { get; set; }
}