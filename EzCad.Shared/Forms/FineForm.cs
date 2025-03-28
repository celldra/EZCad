using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EzCad.Shared.Forms;

public class FineForm : BaseForm
{
    [Required]
    [JsonPropertyName("amount")]
    public double Amount { get; set; }

    [Required]
    [MaxLength(125, ErrorMessage = "Cannot be more than 125 characters")]
    [MinLength(5, ErrorMessage = "Cannot be less than 5 characters")]
    [JsonPropertyName("description")]
    public string Description { get; set; }
}