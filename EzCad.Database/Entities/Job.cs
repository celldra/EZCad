using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EzCad.Database.Entities;

public class Job : BaseEntity
{
    [Required]
    [MaxLength(50)]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [Required]
    [JsonPropertyName("salary")]
    public double Salary { get; set; }

    [Required]
    [JsonPropertyName("isPublic")]
    public bool IsPublic { get; set; }
}