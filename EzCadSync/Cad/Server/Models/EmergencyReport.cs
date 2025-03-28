using System;
using EzCadSync.Api.Models;
using EzCadSync.Shared.Models;
using Newtonsoft.Json;

namespace EzCadSync.Server.Models;

public class EmergencyReport
{
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("dateCreated")] public DateTime DateCreated { get; set; }

    [JsonProperty("description")] public string Description { get; set; }

    [JsonProperty("area")] public string Area { get; set; }

    [JsonProperty("postCode")] public string PostCode { get; set; }

    [JsonProperty("reportingIdentity")] public virtual Identity ReportingIdentity { get; set; }
}