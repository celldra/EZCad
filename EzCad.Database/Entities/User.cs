using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EzCad.Database.Entities;

[Table("Users")]
[Index(nameof(LicenseId), nameof(DiscordId), nameof(Salt), IsUnique = true)]
public class User : IdentityUser
{
    [Required]
    [MaxLength(50)]
    [MinLength(50)]
    [JsonIgnore]
    public string Salt { get; set; }

    [JsonPropertyName("discordId")] public string? DiscordId { get; set; }

    [Required]
    [JsonPropertyName("dateCreated")]
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("licenseId")] public string? LicenseId { get; set; }

    [Required]
    [JsonPropertyName("isLinked")]
    public bool IsLinked { get; set; }

    [Required]
    [JsonPropertyName("banRecords")]
    public virtual List<BanRecord> BanRecords { get; set; } = new();

    [Required]
    [JsonPropertyName("lastBenefitCollection")]
    public DateTime LastBenefitCollection { get; set; } = DateTime.UtcNow;
}