using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;

namespace EzCadSync.Client;

public class ClientMain : BaseScript
{
    public ClientMain()
    {
        Debug.WriteLine("Registering chat suggestions");

        TriggerEvent("chat:addSuggestion", "/link-cad",
            "Link your CAD account with your FiveM account", new[]
            {
                new {name = "id", help = "The ID of your CAD account to link"}
            });

        Debug.WriteLine("Registered chat suggestions");

        Debug.WriteLine("Triggering configuration update");
        TriggerServerEvent("EZCad:TriggerConfigurationUpdate");

        Debug.WriteLine("Triggering officers update");
        TriggerServerEvent("EZCad:TriggerOfficersUpdate");
        
        Debug.WriteLine("Triggering client balance update");
        TriggerServerEvent("EZCad:TriggerBalanceUpdate");
    }
}