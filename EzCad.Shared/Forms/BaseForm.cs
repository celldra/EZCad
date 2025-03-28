using System.Text.Json.Serialization;

namespace EzCad.Shared.Forms;

public abstract class BaseForm
{
    [JsonIgnore] public bool IsProcessing { get; set; }

    [JsonIgnore] public bool IsValid { get; set; }

    [JsonPropertyName("captchaToken")] public string? CaptchaToken { get; set; }
}