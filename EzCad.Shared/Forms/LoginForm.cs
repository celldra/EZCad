using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EzCad.Shared.Forms;

public class LoginForm : BaseForm
{
    [Required(ErrorMessage = "This is required")]
    [MaxLength(25, ErrorMessage = "Cannot be more than 25 characters")]
    [MinLength(2, ErrorMessage = "Cannot be less than 2 characters")]
    [JsonPropertyName("userName")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "This is required")]
    [MaxLength(256, ErrorMessage = "Cannot be more than 256 characters")]
    [MinLength(8, ErrorMessage = "Cannot be less than 8 characters")]
    [JsonPropertyName("password")]
    public string Password { get; set; }
}