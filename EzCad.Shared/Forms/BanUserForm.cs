using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EzCad.Shared.Forms;

public class BanUserForm : BaseForm
{
    [Required(ErrorMessage = "This is required")]
    [MaxLength(255, ErrorMessage = "Cannot be longer than 255 characters")]
    [MinLength(2, ErrorMessage = "Cannot be less than 2 characters")]
    [JsonPropertyName("reason")]
    public string Reason { get; set; }

    [Required(ErrorMessage = "This is required")]
    [JsonPropertyName("isPermanent")]
    public bool IsPermanent { get; set; }

    [JsonPropertyName("expiration")] public DateTime Expiration { get; set; } = DateTime.UtcNow;
}