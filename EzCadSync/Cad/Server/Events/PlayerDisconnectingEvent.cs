using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace EzCadSync.Server.Events;

public class PlayerDisconnectingEvent : BaseScript
{
    [EventHandler("playerDropped")]
    public async void Handle([FromSource] Player player, string reason)
    {
        Debug.WriteLine($"{player.Name} disconnected for reason {reason}");

        var licenseId = player.Identifiers["license"];

        if (!MemoryStorage.AuthorizedPlayers.ContainsKey(licenseId)) return;

        Debug.WriteLine($"Found {player.Name} in player state storage");

        var roles = MemoryStorage.AuthorizedPlayers[licenseId];
        MemoryStorage.AuthorizedIdentities.TryRemove(licenseId, out _);

        Debug.WriteLine($"Revoking {roles.Length} ace permission(s) from {player.Name}");

        var permissionAdd = $"remove_principal identifier.license:{licenseId} group.";
        foreach (var role in roles)
        {
            await Delay(0);

            API.ExecuteCommand($"{permissionAdd}{role}");

            Debug.WriteLine($"Revoked {role} from {player.Name}");
        }

        if (!MemoryStorage.AuthorizedPlayers.TryRemove(licenseId, out _))
            Debug.WriteLine(
                $"Failed to remove {player.Name} from player state storage, this WILL cause ace permission problems for that user");
    }
}