using CitizenFX.Core.Native;
using CitizenFX.Core.UI;

namespace GallagherCommands.Client;

public static class AlertManager
{
    public static void Custom(string message, bool blink = true, bool saveToBrief = true)
    {
        API.SetNotificationTextEntry("CELL_EMAIL_BCON"); // 10x ~a~
        foreach (var s in Screen.StringToArray(message)) API.AddTextComponentSubstringPlayerName(s);
        API.DrawNotification(blink, saveToBrief);
    }
}