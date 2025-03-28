using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EzCad.Shared.Forms;

public class RegisterForm : BaseForm
{
    [Required(ErrorMessage = "This is required")]
    [MaxLength(50, ErrorMessage = "Cannot be more than 50 characters")]
    [MinLength(2, ErrorMessage = "Cannot be less than 2 characters")]
    [DisplayName("Username")]
    [JsonPropertyName("userName")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "This is required")]
    [MaxLength(75, ErrorMessage = "Cannot be more than 75 characters")]
    [MinLength(4, ErrorMessage = "Cannot be less than 4 characters")]
    [DisplayName("Email")]
    [JsonPropertyName("email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "This is required")]
    [MaxLength(256, ErrorMessage = "Cannot be more than 256 characters")]
    [MinLength(8, ErrorMessage = "Cannot be less than 8 characters")]
    [DisplayName("Password")]
    [JsonPropertyName("password")]
    public string Password { get; set; }
}