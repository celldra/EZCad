using System;
using Newtonsoft.Json;

namespace EzCadSync.Client.Models;

public class Identity
{
    [JsonProperty("firstName")] public string FirstName { get; set; }

    [JsonProperty("lastName")] public string LastName { get; set; }

    [JsonProperty("birthPlace")] public string BirthPlace { get; set; }

    [JsonProperty("dateOfBirth")] public DateTime DateOfBirth { get; set; }

    [JsonProperty("balance")] public double Balance { get; set; }
    
    [JsonProperty("sex")] public Sex Sex { get; set; }

    [JsonProperty("isPrimary")] public bool IsPrimary { get; set; }
}