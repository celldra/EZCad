using Newtonsoft.Json;

namespace EzCadSync.Api.Forms;

public class FineForm : BaseForm
{
    [JsonProperty("amount")] public double Amount { get; set; }

    [JsonProperty("description")] public string Description { get; set; }
}