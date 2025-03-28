using CitizenFX.Core;
using EzCadSync.Shared.Models;
using Newtonsoft.Json;

namespace EzCadSync.Client.Events;

public class UpdateConfigurationEvent : BaseScript
{
    [EventHandler("EZCad:UpdateConfiguration")]
    public void Handle(string configurationJson)
    {
        Debug.WriteLine("Updated configuration");

        var configuration = JsonConvert.DeserializeObject<SyncConfiguration>(configurationJson);

        MemoryStorage.Configuration = configuration;
    }
}