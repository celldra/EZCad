using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EzCad.Database.Entities;

public class CriminalRecord : BaseEntity
{
    [Required]
    [JsonPropertyName("offender")]
    public virtual Identity Offender { get; set; }

    [Required]
    [JsonPropertyName("officer")]
    public virtual Identity Officer { get; set; }

    [Required]
    [MaxLength(125)]
    [MinLength(5)]
    [JsonPropertyName("offence")]
    public string Offence { get; set; }

    [Required]
    [MaxLength(50)]
    [MinLength(3)]
    [JsonPropertyName("action")]
    public string Action { get; set; }

    public override string ToString()
    {
        return $"{Offence} {Offender.FirstName} {Offender.LastName} {Officer.FirstName} {Officer.LastName} {Action}";
    }
}