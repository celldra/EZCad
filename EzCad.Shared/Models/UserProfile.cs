using System.Text.Json.Serialization;
using EzCad.Shared.Responses;

namespace EzCad.Shared.Models;

public class UserProfile : BaseResponse
{
    [JsonPropertyName("id")] public string Id { get; set; }
    [JsonPropertyName("username")] public string UserName { get; set; }
    [JsonPropertyName("dateCreated")] public DateTime DateCreated { get; set; }
    [JsonPropertyName("email")] public string Email { get; set; }
    [JsonPropertyName("roles")] public string[] Roles { get; set; }
    [JsonPropertyName("banRecords")] public List<BanResponse> BanRecords { get; set; }
    [JsonPropertyName("isLinked")] public bool IsLinked { get; set; }
    [JsonPropertyName("license")] public string? License { get; set; }

    [JsonPropertyName("discordId")] public ulong? DiscordId { get; set; }

    [JsonPropertyName("lastBenefitsCollection")]
    public DateTime LastBenefitsCollection { get; set; }

    public override string ToString()
    {
        return $"{Email} {License} {UserName} {DateCreated} {Id}";
    }
}