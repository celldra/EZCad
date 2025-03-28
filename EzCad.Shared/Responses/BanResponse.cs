using System.Text.Json.Serialization;

namespace EzCad.Shared.Responses;

public class BanResponse : ErrorResponse
{
    [JsonPropertyName("reason")] public string Reason { get; set; }

    [JsonPropertyName("bannedBy")] public string BannedBy { get; set; }

    [JsonPropertyName("isPermanent")] public bool IsPermanent { get; set; }

    [JsonPropertyName("expiration")] public DateTime? Expiration { get; set; }

    [JsonPropertyName("dateBanned")] public DateTime? DateBanned { get; set; }
}