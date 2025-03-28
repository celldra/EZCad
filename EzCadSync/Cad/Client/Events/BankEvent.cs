using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;

namespace EzCadSync.Client.Events;

public class BankEvent : BaseScript
{
    [EventHandler("EZCad:BankNotify")]
    public void OnBankNotify(string subject, string message)
    {
        API.SetNotificationTextEntry("CELL_EMAIL_BCON"); // 10x ~a~
        foreach (var s in Screen.StringToArray(message)) API.AddTextComponentSubstringPlayerName(s);
        API.SetNotificationMessage("CHAR_BANK_MAZE", "CHAR_BANK_MAZE", true, 0, "Maze Bank", subject);
        API.DrawNotification(false, true);
    }
}