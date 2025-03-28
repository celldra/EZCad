using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EzCad.Shared.Forms;

public class BalanceForm : BaseForm
{
    [JsonPropertyName("balance")]
    [Required]
    public double Balance { get; set; }
}