using Newtonsoft.Json;

namespace EzCadSync.Api.Forms;

public class SendMoneyForm : BaseForm
{
    [JsonProperty("toLicenseId")] public string ToLicenseId { get; set; }

    [JsonProperty("amount")] public double Amount { get; set; }

    [JsonProperty("description")] public string Description { get; set; }
}