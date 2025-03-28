using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EzCad.Shared.Forms;

public class GameLoginForm : BaseForm
{
    [Required]
    [MaxLength(100, ErrorMessage = "Cannot be longer than 100 characters")]
    [JsonPropertyName("license")]
    public string License { get; set; }

    [Required]
    [MaxLength(255, ErrorMessage = "Cannot be longer than 255 characters")]
    [JsonPropertyName("name")]
    public string Name { get; set; }
}