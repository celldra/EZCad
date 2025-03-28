using Newtonsoft.Json;

namespace EzCadSync.Api.Forms;

public class EmergencyReportForm : BaseForm
{
    [JsonProperty("description")] public string Description { get; set; }

    [JsonProperty("area")] public string Area { get; set; }

    [JsonProperty("postCode")] public string PostCode { get; set; }

    [JsonProperty("reporterLicenseId")] public string ReporterLicenseId { get; set; }
}