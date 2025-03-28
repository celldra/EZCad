using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EzCad.Shared.Forms;

public class ConfigurationForm : BaseForm
{
    [Required(ErrorMessage = "This is required")]
    [MaxLength(50, ErrorMessage = "Cannot be more than 50 characters")]
    [MinLength(1, ErrorMessage = "Cannot be less than 1 character")]
    [JsonPropertyName("serverName")]
    public string ServerName { get; set; }

    [Required(ErrorMessage = "This is required")]
    [RegularExpression("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Must be a valid hexadecimal color code")]
    [JsonPropertyName("hexColor")]
    public string HexColor { get; set; }

    [Required(ErrorMessage = "This is required")]
    [MaxLength(1, ErrorMessage = "Cannot be more than 1 character")]
    [MinLength(1, ErrorMessage = "Cannot be less than 1 character")]
    [JsonPropertyName("currency")]
    public string Currency { get; set; }

    [Required(ErrorMessage = "This is required")]
    [MaxLength(100, ErrorMessage = "Cannot be more than 100 characters")]
    [MinLength(1, ErrorMessage = "Cannot be less than 1 character")]
    [JsonPropertyName("serverConnectUrl")]
    public string ServerConnectUrl { get; set; }
}