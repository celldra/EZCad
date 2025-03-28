using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EzCad.Database.Entities;

public class Transaction : BaseEntity
{
    [Required]
    [MaxLength(75)]
    [MinLength(2)]
    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("amount")]
    [Required]
    public double Amount { get; set; }

    [JsonPropertyName("toIdentity")] public virtual Identity? ToIdentity { get; set; }

    [JsonPropertyName("fromIdentity")] public virtual Identity? FromIdentity { get; set; }

    [Required]
    [JsonPropertyName("increase")]
    public bool Increase { get; set; }

    [Required]
    [JsonPropertyName("hostIdentity")]
    public virtual Identity HostIdentity { get; set; }
}