using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using GallagherCommands.Shared;

namespace GallagherCommands.Client.Handlers;

public class GamerTagHandler : BaseScript
{
    private readonly Dictionary<Player, int> _gamerTags = new();
    private const float GamerTagDistance = 1000f;

    [Tick]
    public async Task HandleAsync()
    {
        if (MemoryStorage.IsDisablingGamertags)
        {
            // Remove gamertags
            Debug.WriteLine("Removing gamertags");
            foreach (var player in Players.Where(x => x != Game.Player))
            {
                if (!_gamerTags.ContainsKey(player)) continue;

                API.RemoveMpGamerTag(_gamerTags[player]);
                _gamerTags.Remove(player);
            }

            // Reset state and return
            MemoryStorage.IsDisablingGamertags = false;
            return;
        }
        
        if (!MemoryStorage.IsShowingGamertags) return;

        await Delay(500);

        foreach (var player in Players.Where(x => x != Game.Player))
        {
            var dist = player.Character.Position.DistanceToSquared(Game.PlayerPed.Position);
            var closeEnough = dist < GamerTagDistance;
            if (_gamerTags.ContainsKey(player))
            {
                if (!closeEnough)
                {
                    API.RemoveMpGamerTag(_gamerTags[player]);
                    _gamerTags.Remove(player);
                }
                else
                {
                    _gamerTags[player] = API.CreateMpGamerTag(player.Character.Handle,
                        player.Name + $" [{player.ServerId}]", false,
                        false, "", 0);
                }
            }
            else if (closeEnough)
            {
                _gamerTags.Add(player,
                    API.CreateMpGamerTag(player.Character.Handle, player.Name + $" [{player.ServerId}]", false, false,
                        string.Empty, 0));
            }

            if (!closeEnough || !_gamerTags.ContainsKey(player)) continue;

            API.SetMpGamerTagVisibility(_gamerTags[player], 2, true); // healthArmor
            if (player.WantedLevel > 0)
            {
                API.SetMpGamerTagVisibility(_gamerTags[player], 7, true); // wantedStars
                API.SetMpGamerTagWantedLevel(_gamerTags[player], API.GetPlayerWantedLevel(player.Handle));
            }
            else
            {
                API.SetMpGamerTagVisibility(_gamerTags[player], 7, false); // wantedStars
            }
        }
    }
}