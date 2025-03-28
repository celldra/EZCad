using CitizenFX.Core;

namespace EzCadSync.Client.Events;

public class OnMapStartedEvent : BaseScript
{
    [EventHandler("onClientMapStart")]
    public async void OnClientMapStart()
    {
        Debug.WriteLine("Map started on client");
        
        Exports["spawnmanager"].setAutoSpawn(false);
        
        await Delay(2500);
        
        Debug.WriteLine("Spawning player");
        
        Exports["spawnmanager"].spawnPlayer();
        
        await Delay(5 * 1000);
        
        Debug.WriteLine("Triggering onClientMapStart initial balance request");
        TriggerServerEvent("EZCad:TriggerBalanceUpdate");
    }
}