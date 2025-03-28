using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EzCad.Database.Entities;

public class BanRecord : BaseEntity
{
    [Required]
    [MaxLength(255)]
    [MinLength(2)]
    [JsonPropertyName("reason")]
    public string Reason { get; set; }

    [Required] [JsonIgnore] public virtual User BannedBy { get; set; }

    [Required]
    [JsonPropertyName("isPermanent")]
    public bool IsPermanent { get; set; }

    [Required]
    [JsonPropertyName("expiration")]
    public DateTime Expiration { get; set; } = DateTime.MaxValue;
}