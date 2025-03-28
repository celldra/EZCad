using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using EzCad.Database.Models;

namespace EzCad.Shared.Forms;

public class IdentityForm : BaseForm
{
    [Required(ErrorMessage = "This is required")]
    [MaxLength(25, ErrorMessage = "Cannot be longer than 25 characters")]
    [MinLength(2, ErrorMessage = "Cannot be less than 2 characters")]
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "This is required")]
    [MaxLength(25, ErrorMessage = "Cannot be longer than 25 characters")]
    [MinLength(2, ErrorMessage = "Cannot be less than 2 characters")]
    [JsonPropertyName("lastName")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "This is required")]
    [MaxLength(50)]
    [MinLength(2, ErrorMessage = "Cannot be less than 2 characters")]
    [JsonPropertyName("birthPlace")]
    public string BirthPlace { get; set; }

    [Required(ErrorMessage = "This is required")]
    [JsonPropertyName("dateOfBirth")]
    public DateTime DateOfBirth { get; set; } = DateTime.UtcNow;

    [Required(ErrorMessage = "This is required")]
    [JsonPropertyName("isPrimary")]
    public bool IsPrimary { get; set; }
    
    [Required(ErrorMessage = "This is required")]
    [JsonPropertyName("sex")]
    public Sex Sex { get; set; }
    
    [Required(ErrorMessage = "This is required")]
    [JsonPropertyName("weaponsLicense")]
    public LicenseState WeaponsLicense { get; set; }
    
    [Required(ErrorMessage = "This is required")]
    [JsonPropertyName("huntingLicense")]
    public LicenseState HuntingLicense { get; set; }

    [Required(ErrorMessage = "This is required")]
    [JsonPropertyName("drivingLicense")]
    public LicenseState DrivingLicense { get; set; }
}