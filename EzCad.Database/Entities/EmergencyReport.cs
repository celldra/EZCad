using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EzCad.Database.Entities;

public class EmergencyReport : BaseEntity
{
    [Required]
    [MaxLength(255)]
    [MinLength(2)]
    [JsonPropertyName("description")]
    public string Description { get; set; }

    [Required]
    [MaxLength(255)]
    [MinLength(2)]
    [JsonPropertyName("area")]
    public string Area { get; set; }

    [Required]
    [JsonPropertyName("postCode")]
    public string PostCode { get; set; }

    [Required]
    [JsonPropertyName("reportingIdentity")]
    public virtual Identity ReportingIdentity { get; set; }
}