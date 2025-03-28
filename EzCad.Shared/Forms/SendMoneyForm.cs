using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EzCad.Shared.Forms;

public class SendMoneyForm : BaseForm
{
    [Required]
    [MaxLength(100, ErrorMessage = "Cannot be longer than 100 characters")]
    [JsonPropertyName("toLicenseId")]
    public string ToLicenseId { get; set; }

    [Required]
    [JsonPropertyName("amount")]
    public double Amount { get; set; }

    [Required]
    [MaxLength(75, ErrorMessage = "Cannot be longer than 100 characters")]
    [MinLength(2, ErrorMessage = "Cannot be less than 2 characters")]
    [JsonPropertyName("description")]
    public string Description { get; set; }
}