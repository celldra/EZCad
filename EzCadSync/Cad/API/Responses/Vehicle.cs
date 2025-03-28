using EzCadSync.Api.Models;
using Newtonsoft.Json;

namespace EzCadSync.Api.Responses;

public class Vehicle
{
    [JsonProperty("licensePlate")] public string LicensePlate { get; set; }

    [JsonProperty("model")] public string Model { get; set; }

    [JsonProperty("manufacturer")] public string Manufacturer { get; set; }

    [JsonProperty("isStolen")] public bool IsStolen { get; set; }

    [JsonProperty("motState")] public int MotState { get; set; }

    [JsonProperty("insuranceState")] public int InsuranceState { get; set; }

    [JsonProperty("hostIdentity")] public Identity HostIdentity { get; set; }
}