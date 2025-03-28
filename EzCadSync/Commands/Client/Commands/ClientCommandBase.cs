using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace GallagherCommands.Client.Commands;

public abstract class ClientCommandBase : BaseScript
{
    protected void ThrowNoPermission()
    {
        SendChatMessage("You don't have permission to run this command!");
    }

    protected async Task PlayAnimationFromDictionaryAsync(Ped ped, string dict, string name)
    {
        if (!API.DoesAnimDictExist(dict))
        {
            SendChatMessage($"{dict} does not exist as an animation dictionary");
            return;
        }

        API.RequestAnimDict(dict);

        while (!API.HasAnimDictLoaded(dict)) await Delay(0);

        API.TaskPlayAnim(ped.Handle, dict, name, 8f, -8f, -1, 270540800, 0, false, false, false);

        if (dict.Equals("mp_suicide", StringComparison.OrdinalIgnoreCase))
        {
            var shot = false;
            while (true)
            {
                var time = API.GetEntityAnimCurrentTime(Game.PlayerPed.Handle, "MP_SUICIDE",
                    name.Contains("pill") ? "pill" : "pistol");
                
                if (API.HasAnimEventFired(Game.PlayerPed.Handle, (uint) API.GetHashKey("Fire")) &&
                    !shot) // shoot the gun if the animation event is triggered.
                {
                    API.ClearEntityLastDamageEntity(Game.PlayerPed.Handle);
                    API.SetPedShootsAtCoord(Game.PlayerPed.Handle, 0f, 0f, 0f, false);
                    shot = true;
                }

                if (time > (name.Contains("pill") ? 0.536f : 0.365f))
                    // Return now that we know the animation is finished
                    return;
                await Delay(0);
            }
        }

        API.RemoveAnimDict(dict);
    }

    protected void SendChatMessage(string data)
    {
        TriggerEvent("chat:addMessage", new
        {
            multiline = true,
            color = new[] {255, 255, 255},
            args = new[] {"System", data}
        });
    }

    protected bool HasNoArguments(List<object> args)
    {
        return args.Count <= 0;
    }

    public virtual void RunCommand(int source, List<object> args, string raw)
    {
        throw new NotImplementedException();
    }
}