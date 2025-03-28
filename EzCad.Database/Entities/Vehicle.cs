using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using EzCad.Database.Models;

namespace EzCad.Database.Entities;

[Table("Vehicles")]
public class Vehicle : BaseEntity
{
    [Required]
    [MaxLength(7, ErrorMessage = "Cannot be longer than 7 characters")]
    [JsonPropertyName("licensePlate")]
    public string LicensePlate { get; set; }

    [Required]
    [MaxLength(100, ErrorMessage = "Cannot be longer than 100 characters")]
    [JsonPropertyName("model")]
    public string Model { get; set; }

    [Required]
    [MaxLength(100, ErrorMessage = "Cannot be longer than 100 characters")]
    [JsonPropertyName("manufacturer")]
    public string Manufacturer { get; set; }

    [Required]
    [JsonPropertyName("isStolen")]
    public bool IsStolen { get; set; }

    [Required]
    [JsonPropertyName("motState")]
    public LicenseState MotState { get; set; } = LicenseState.Valid;

    [Required]
    [JsonPropertyName("insuranceState")]
    public LicenseState InsuranceState { get; set; } = LicenseState.Valid;

    [Required]
    [JsonPropertyName("hostIdentity")]
    public virtual Identity HostIdentity { get; set; }
}