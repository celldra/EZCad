using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;

namespace EzCadSync.Client.Events;

public class SetBalanceEvent : BaseScript
{
    [EventHandler("EZCad:SetBalance")]
    public void OnSetBalance(double balance)
    {
        Debug.WriteLine($"Setting client balance to {balance}");

        MemoryStorage.Balance = balance;
        
        var idMessage = new
        {
            type = "id",
            id = Game.Player.ServerId
        };

        var message = new
        {
            type = "balance",
            balance = MemoryStorage.Balance
        };

        API.SendNuiMessage(JsonConvert.SerializeObject(idMessage));
        API.SendNuiMessage(JsonConvert.SerializeObject(message));
    }
}