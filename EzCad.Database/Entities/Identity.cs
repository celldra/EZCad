using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using EzCad.Database.Models;

namespace EzCad.Database.Entities;

[Table("Identities")]
public class Identity : BaseEntity
{
    [Required]
    [MaxLength(25)]
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(25)]
    [JsonPropertyName("lastName")]
    public string LastName { get; set; }

    [Required]
    [MaxLength(50)]
    [MinLength(2)]
    [JsonPropertyName("birthPlace")]
    public string BirthPlace { get; set; }

    [Required]
    [JsonPropertyName("dateOfBirth")]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [JsonPropertyName("isPrimary")]
    public bool IsPrimary { get; set; }

    [Required] [JsonPropertyName("sex")] public Sex Sex { get; set; }

    [Required] [JsonPropertyName("money")] public double Money { get; set; }

    [Required]
    [JsonPropertyName("weaponsLicense")]
    public LicenseState WeaponsLicense { get; set; }

    [Required]
    [JsonPropertyName("huntingLicense")]
    public LicenseState HuntingLicense { get; set; }

    [Required]
    [JsonPropertyName("drivingLicense")]
    public LicenseState DrivingLicense { get; set; }

    [JsonPropertyName("jobId")] public string? JobId { get; set; }

    [Required]
    [JsonPropertyName("hostUser")]
    public virtual User HostUser { get; set; }
}