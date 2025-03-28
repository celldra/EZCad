using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using GallagherCommands.Shared;
using GallagherCommands.Shared.Models;
using Newtonsoft.Json;

namespace GallagherCommands.Client.Events;

public class UpdateEvent : BaseScript
{
    private bool _firstAopUpdate = true;
    
    [EventHandler("GCMD:UpdateAOP")]
    private void UpdateAop(string newAop)
    {

        Debug.WriteLine($"Updating AOP to {newAop}");
        
        if (_firstAopUpdate)
        {
            _firstAopUpdate = false;
        }
        else
        {
            TriggerEvent("UL:ScreenNotify", "AOP updated", $"The AOP has been updated to ~y~{newAop}~s~, you need to move accordingly!");
        }
        
        var message = new
        {
            type = "aop",
            aop = newAop
        };

        API.SendNuiMessage(JsonConvert.SerializeObject(message));
        
        MemoryStorage.DefaultAop = newAop;
    }

    [EventHandler("GCMD:UpdatePT")]
    public void UpdatePeacetime(bool toggle)
    {
        Debug.WriteLine($"Updating peacetime to {toggle}");
        
        MemoryStorage.IsPeaceTimeEnabled = toggle;
        
        if (!toggle) ExtensionMethods.SetPedRelationship();

        Screen.ShowNotification(
            toggle
                ? "Peacetime is now enabled, violent actions are disabled!"
                : "Peacetime is now disabled, watch your back!", true);
    }

    [EventHandler("GCMD:UpdatePriority")]
    public void UpdatePriority(PriorityStatus status)
    {
        Debug.WriteLine($"Updating priority to {status}");
        MemoryStorage.PriorityStatus = status;
    }
}