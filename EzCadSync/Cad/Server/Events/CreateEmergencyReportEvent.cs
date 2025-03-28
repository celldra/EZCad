using CitizenFX.Core;
using CitizenFX.Core.Native;
using EzCadSync.Api.Exceptions;

namespace EzCadSync.Server.Events;

public class CreateEmergencyReportEvent : BaseApiEvent
{
    [EventHandler("EZCad:CreateEmergencyReport")]
    public async void Handle([FromSource] Player player, string description, string area, string postal, float x,
        float y, float z)
    {
        try
        {
            var licenseId = player.Identifiers["license"];

            var response = await Api.CreateEmergencyReportAsync(licenseId, description, area, postal);
            var report = response?.Entity;

            TriggerClientEvent(player, "chat:addMessage", new
            {
                multiline = true,
                color = new[] { 255, 255, 255 },
                args = new[] { "System", "Report has been created, allow up to 5 minutes for EMS to respond" }
            });


            foreach (var localPlayer in Players)
            {
                if (!API.IsPlayerAceAllowed(localPlayer.Handle, "EZCad.GetIdentity")) continue;

                TriggerClientEvent(localPlayer, "EZCad:EmergencyNotify", "Emergency report received",
                    $"Reporter: <C>{report?.ReportingIdentity?.LastName}, {report?.ReportingIdentity?.FirstName}</C>" +
                    $"\nLocation: <C>{report?.Area} ({report?.PostCode})</C>" +
                    $"\nDescription: <C>{report?.Description}</C>");

                TriggerClientEvent(localPlayer, "EZCad:DrawEmergency", x, y, z);
            }
        }
        catch (ApiException ex)
        {
            TriggerClientEvent(player, "chat:addMessage", new
            {
                multiline = true,
                color = new[] { 255, 255, 255 },
                args = new[]
                    { "System", ex.Message }
            });
        }
        finally
        {
            Dispose();
        }
    }
}