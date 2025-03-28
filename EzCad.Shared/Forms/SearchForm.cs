using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EzCad.Shared.Forms;

public class SearchForm : BaseForm
{
    [Required(ErrorMessage = "This is required")]
    [MaxLength(100, ErrorMessage = "Cannot be more than 100 characters")]
    [MinLength(1, ErrorMessage = "Cannot be less than 1 character")]
    [JsonPropertyName("query")]
    public string Query { get; set; }
}