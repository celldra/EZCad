using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EzCad.Shared.Forms;

public class JobForm : BaseForm
{
    [Required(ErrorMessage = "This is required")]
    [MaxLength(50, ErrorMessage = "Cannot be longer than 50 characters")]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [Required(ErrorMessage = "This is required")]
    [JsonPropertyName("salary")]
    public string Salary { get; set; }

    [Required(ErrorMessage = "This is required")]
    [JsonPropertyName("isPublic")]
    public bool IsPublic { get; set; }
}