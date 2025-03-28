using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EzCad.Database.Entities;

public class Login : BaseEntity
{
    [Required]
    [MaxLength(255)]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("hostUser")]
    [Required]
    public virtual User HostUser { get; set; }

    public override string ToString()
    {
        return $"{Name} {HostUser.UserName} {HostUser.LicenseId} {HostUser.Email}";
    }
}