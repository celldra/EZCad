using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;

namespace EzCadSync.Client.Events;

public class EmergencyEvent : BaseScript
{
    [EventHandler("EZCad:EmergencyNotify")]
    public void OnEmergencyNotify(string subject, string message)
    {
        API.SetNotificationTextEntry("CELL_EMAIL_BCON"); // 10x ~a~
        foreach (var s in Screen.StringToArray(message)) API.AddTextComponentSubstringPlayerName(s);
        API.SetNotificationMessage("CHAR_CALL911", "CHAR_CALL911", true, 0, "Dispatch", subject);
        API.DrawNotification(false, true);
    }
}