using System;
using Newtonsoft.Json;

namespace EzCadSync.Api.Responses;

public class BanResponse : ErrorResponse
{
    [JsonProperty("reason")] public string Reason { get; set; }

    [JsonProperty("bannedBy")] public string BannedBy { get; set; }

    [JsonProperty("isPermanent")] public bool IsPermanent { get; set; }

    [JsonProperty("expiration")] public DateTime? Expiration { get; set; }
}