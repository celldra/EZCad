using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace EzCadSync.Server.Events;

public class AuthorizeEvent : BaseScript
{
    [EventHandler("EZCad:Authorize")]
    public async void Handle([FromSource] Player player, string obj, NetworkCallbackDelegate callback)
    {
        if (API.IsPlayerAceAllowed(player.Handle, obj)) await callback(true);
        else await callback(false);
    }
}