using System.Collections.Generic;
using CitizenFX.Core;
using EzCadSync.Shared.Models;
using Newtonsoft.Json;

namespace EzCadSync.Client.Events;

public class UpdateOfficersEvent : BaseScript
{
    [EventHandler("EZCad:UpdateOfficers")]
    public void Handle(string officersJson)
    {
        Debug.WriteLine("Updated officers list");

        var list = JsonConvert.DeserializeObject<Dictionary<string, PlayerIdentity>>(officersJson);

        MemoryStorage.OnDutyIdentities = list;
    }
}