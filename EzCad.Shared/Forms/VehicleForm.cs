using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using EzCad.Database.Models;

namespace EzCad.Shared.Forms;

public class VehicleForm : BaseForm
{
    [Required(ErrorMessage = "This is required")]
    [MaxLength(7, ErrorMessage = "Cannot be longer than 7 characters")]
    [JsonPropertyName("licensePlate")]
    public string LicensePlate { get; set; }

    [Required(ErrorMessage = "This is required")]
    [MaxLength(100, ErrorMessage = "Cannot be longer than 100 characters")]
    [JsonPropertyName("model")]
    public string Model { get; set; }

    [Required(ErrorMessage = "This is required")]
    [MaxLength(100, ErrorMessage = "Cannot be longer than 100 characters")]
    [JsonPropertyName("manufacturer")]
    public string Manufacturer { get; set; }

    [Required]
    [JsonPropertyName("motState")]
    public LicenseState MotState { get; set; } = LicenseState.Valid;

    [Required]
    [JsonPropertyName("insuranceState")]
    public LicenseState InsuranceState { get; set; } = LicenseState.Valid;

    [Required(ErrorMessage = "This is required")]
    [JsonPropertyName("identityId")]
    public string IdentityId { get; set; }

    [Required(ErrorMessage = "This is required")]
    [JsonPropertyName("isStolen")]
    public bool IsStolen { get; set; }
}