using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EzCad.Database.Entities;

public class Configuration : BaseEntity
{
    [Required]
    [JsonPropertyName("primaryHexColor")]
    public string PrimaryHexColor { get; set; } = "#B4656F";

    [Required]
    [JsonPropertyName("serverName")]
    public string ServerName { get; set; } = "EZCad";

    [Required]
    [JsonPropertyName("connectUrl")]
    public string ConnectUrl { get; set; } = "https://cfx.re";

    [JsonPropertyName("currency")]
    [Required]
    public string Currency { get; set; } = "$";
}