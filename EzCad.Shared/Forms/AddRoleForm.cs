using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EzCad.Shared.Forms;

public class AddRoleForm : BaseForm
{
    [Required(ErrorMessage = "This is required")]
    [MaxLength(255, ErrorMessage = "Cannot be longer than 255 characters")]
    [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9]*$", ErrorMessage = "Cannot contain any spaces or special characters")]
    [JsonPropertyName("role")]
    public string Role { get; set; }
}