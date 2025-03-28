using CitizenFX.Core;
using CitizenFX.Core.Native;
using GallagherCommands.Configuration.Models;
using GallagherCommands.Shared;

namespace GallagherCommands.Client.Events;

public class LoadEvent : BaseScript
{    
    private readonly CommandsConfiguration? _configuration = ConfigurationManager.Load();

    [EventHandler("onClientMapStart")]
    public void OnClientMapStart()
    {
        Debug.WriteLine("Updating GCMD state");
        
        TriggerServerEvent("GCMD:Update");
    }
}