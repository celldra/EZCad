using Newtonsoft.Json;

namespace EzCadSync.Api.Forms;

public class GameLoginForm : BaseForm
{
    [JsonProperty("license")] public string License { get; set; }

    [JsonProperty("name")] public string Name { get; set; }
}