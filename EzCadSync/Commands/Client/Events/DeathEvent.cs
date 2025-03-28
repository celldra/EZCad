using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using GallagherCommands.Configuration.Models;
using GallagherCommands.Shared;

namespace GallagherCommands.Client.Events;

public class DeathEvent : BaseScript
{
    private readonly CommandsConfiguration? _configuration;
    private int _deathTimer;
    private bool _alertGiven;
    private bool _respawnEventGiven;
    private static bool _soundLock;

    public DeathEvent()
    {
        _alertGiven = false;
        _respawnEventGiven = false;
        _soundLock = false;
        _configuration = ConfigurationManager.Load();
        if (_configuration != null) _deathTimer = _configuration.RespawnInterval;
    }

    [EventHandler("GCMD:Respawn")]
    private void RunRespawnEvent(int playerId, bool bypassTimer)
    {
        var player = Players[playerId];
        var ped = player.Character;
        
        if (!bypassTimer && !_respawnEventGiven)
        {
            TriggerEvent("chat:addMessage",
                new
                {
                    multiline = true,
                    color = new[] {255, 255, 255},
                    args = new[] {"System", $"You need to wait ^5{_deathTimer} more seconds!"}
                });
            return;
        }

        API.NetworkResurrectLocalPlayer(ped.Position.X, ped.Position.Y, ped.Position.Z, ped.Heading, true, false);
        API.ClearPedBloodDamage(ped.Handle);
        API.SetEntityCoords(ped.Handle, (float) _configuration.RespawnCoOrdinates.X,
            (float) _configuration.RespawnCoOrdinates.Y,
            (float) _configuration.RespawnCoOrdinates.Z, false, false, false, false);
    }

    [EventHandler("GCMD:Revive")]
    private void RunReviveEvent(int playerId, bool bypassTimer)
    {
        var player = Players[playerId];
        var ped = player.Character;
        
        if (!API.IsEntityDead(ped.Handle))
        {
            TriggerEvent("chat:addMessage",
                new
                {
                    multiline = true,
                    color = new[] {255, 255, 255},
                    args = new[] {"System", "Unfortunately, you can't revive someone who's not dead..."}
                });
            return;
        }

        if (!bypassTimer && !_respawnEventGiven)
        {
            TriggerEvent("chat:addMessage",
                new
                {
                    multiline = true,
                    color = new[] {255, 255, 255},
                    args = new[] {"System", $"You need to wait ^5{_deathTimer} more seconds!"}
                });
            return;
        }

        API.NetworkResurrectLocalPlayer(ped.Position.X, ped.Position.Y, ped.Position.Z, ped.Heading, true, false);
        API.ClearPedBloodDamage(ped.Handle);
    }

    [Tick]
    public async Task WastedScreenAsync()
    {
        var player = API.PlayerId();
        if (!API.IsPlayerDead(player) || _configuration?.EnableWastedScreen != true) return;

        // This plays the wasted screen if enabled
        API.StartScreenEffect("DeathFailOut", 0, false);
        if (!_soundLock)
        {
            API.PlaySoundFrontend(-1, "Bed", "WastedSounds", true);
            _soundLock = true;
        }

        API.ShakeGameplayCam("DEATH_FAIL_IN_EFFECT_SHAKE", 1.0f);

        var scaleForm = API.RequestScaleformMovie("MP_BIG_MESSAGE_FREEMODE");

        while (!API.HasScaleformMovieLoaded(scaleForm)) await Delay(0);

        API.BeginScaleformMovieMethod(scaleForm, "SHOW_SHARD_WASTED_MP_MESSAGE");
        API.PushScaleformMovieMethodParameterString("~r~wasted");
        API.PushScaleformMovieMethodParameterString("You died.");
        API.PushScaleformMovieMethodParameterInt(5);
        API.EndScaleformMovieMethod();

        await Delay(500);

        API.PlaySoundFrontend(-1, "TextHit", "WastedSounds", true);

        while (API.IsPlayerDead(player))
        {
            API.DrawScaleformMovieFullscreen(scaleForm, 255, 255, 255, 255, 0);
            await Delay(0);
        }

        API.StopScreenEffect("DeathFailOut");
        _soundLock = false;
    }


    [Tick]
    public async Task WaitForDeathAsync()
    {
        var player = API.PlayerId();

        if (!API.IsPlayerDead(player) && _respawnEventGiven)
        {
            // This is if the respawn event is given and they are no longer dead
            _respawnEventGiven = false;
            return;
        }

        if (!API.IsPlayerDead(player)) return;

        if (!_alertGiven)
        {
            Debug.WriteLine("Trigger death alert");
            Screen.ShowNotification($"You've died, you must wait {_deathTimer} second(s) before respawning", true);
            _alertGiven = true;
        }

        if (API.IsPlayerDead(player) && _deathTimer > 0)
        {
            await Delay(1000);
            _deathTimer--;
        }
        else if (!_respawnEventGiven)
        {
            Debug.WriteLine("Trigger respawn alert");
            Screen.ShowNotification($"You can respawn now, use either the <C>/respawn</C> or <C>/revive</C> commands in chat to respawn or revive yourself", true);
            
            _respawnEventGiven = true;
            _deathTimer = _configuration.RespawnInterval;
        }
    }
}