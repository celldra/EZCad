using Newtonsoft.Json;

namespace EzCadSync.Api.Forms;

public abstract class BaseForm
{
    [JsonIgnore] public bool IsProcessing { get; set; }

    [JsonIgnore] public bool IsValid { get; set; }
}