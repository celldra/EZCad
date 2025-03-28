using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EzCad.Database.Entities;

public class BaseEntity
{
    [Key]
    [Required]
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [JsonPropertyName("dateCreated")]
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
}