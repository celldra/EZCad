using Newtonsoft.Json;

namespace EzCadSync.Api.Forms;

public class CriminalRecordForm : BaseForm
{
    [JsonProperty("offenderLicenseId")] public string OffenderLicenseId { get; set; }

    [JsonProperty("officerLicenseId")] public string OfficerLicenseId { get; set; }

    [JsonProperty("offence")] public string Offence { get; set; }

    [JsonProperty("action")] public string Action { get; set; }
}