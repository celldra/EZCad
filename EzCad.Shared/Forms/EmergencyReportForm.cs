using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EzCad.Shared.Forms;

public class EmergencyReportForm : BaseForm
{
    [Required(ErrorMessage = "This is required")]
    [MaxLength(255, ErrorMessage = "Cannot be more than 255 characters")]
    [MinLength(2, ErrorMessage = "Cannot be less than 2 characters")]
    [JsonPropertyName("description")]
    public string Description { get; set; }

    [Required(ErrorMessage = "This is required")]
    [MaxLength(255, ErrorMessage = "Cannot be more than 255 characters")]
    [MinLength(2, ErrorMessage = "Cannot be less than 2 characters")]
    [JsonPropertyName("area")]
    public string Area { get; set; }

    [Required(ErrorMessage = "This is required")]
    [JsonPropertyName("postCode")]
    public string PostCode { get; set; }

    [Required(ErrorMessage = "This is required")]
    [JsonPropertyName("reporterLicenseId")]
    public string ReporterLicenseId { get; set; }
}