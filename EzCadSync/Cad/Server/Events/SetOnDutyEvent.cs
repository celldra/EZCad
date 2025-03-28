using CitizenFX.Core;
using CitizenFX.Core.Native;
using EzCadSync.Shared.Models;
using Newtonsoft.Json;

namespace EzCadSync.Server.Events;

public class SetOnDutyEvent : BaseScript
{
    [EventHandler("EZCad:SetOnDuty")]
    public void Handle([FromSource] Player player, bool state)
    {
        var license = player.Identifiers["license"];

        if (!MemoryStorage.AuthorizedIdentities.ContainsKey(license))
        {
            TriggerClientEvent(player, "EZCad:EmergencyNotify", "Go on/off duty",
                "Failed to find your identity in the identities storage, please exit the game and re-connect to the server for the CAD syncing script to store your identity.");
            return;
        }

        var identity = MemoryStorage.AuthorizedIdentities[license];

        // Convert identity to a player identity so we can fetch the related player on the client side
        var playerIdentity = new PlayerIdentity
        {
            Balance = identity.Balance,
            FirstName = identity.FirstName,
            IsPrimary = identity.IsPrimary,
            LastName = identity.LastName,
            BirthPlace = identity.BirthPlace,
            Name = player.Name,
            DateOfBirth = identity.DateOfBirth
        };

        if (state)
        {
            Debug.WriteLine("Officer set on duty");
            MemoryStorage.OnDutyIdentities[license] = playerIdentity;

            Exports["rp-radio"].GivePlayerAccessToFrequencies(1, 2, 3, 4);
        }
        else
        {
            Debug.WriteLine("Officer set off duty");
            MemoryStorage.OnDutyIdentities.TryRemove(license, out _);
        }

        var dutyJson = JsonConvert.SerializeObject(MemoryStorage.OnDutyIdentities);

        foreach (var p in Players)
        {
            if (!API.IsPlayerAceAllowed(p.Handle, "EZCad.CreateRecord")) continue;

            TriggerClientEvent(p, "EZCad:UpdateOfficers", dutyJson);
        }
    }
}