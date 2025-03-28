using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EzCad.Shared.Forms;

public class CriminalRecordForm : BaseForm
{
    [Required(ErrorMessage = "This is required")]
    [JsonPropertyName("offenderLicenseId")]
    public string OffenderLicenseId { get; set; }

    [Required(ErrorMessage = "This is required")]
    [JsonPropertyName("officerLicenseId")]
    public string OfficerLicenseId { get; set; }

    [Required(ErrorMessage = "This is required")]
    [MaxLength(125, ErrorMessage = "Cannot be more than 125 characters")]
    [MinLength(5, ErrorMessage = "Cannot be less than 5 characters")]
    [JsonPropertyName("offence")]
    public string Offence { get; set; }

    [Required(ErrorMessage = "This is required")]
    [MaxLength(50, ErrorMessage = "Cannot be more than 50 characters")]
    [MinLength(3, ErrorMessage = "Cannot be less than 3 characters")]
    [JsonPropertyName("action")]
    public string Action { get; set; }
}