using CitizenFX.Core;
using CitizenFX.Core.Native;
using EzCadSync.Shared.Models;
using Newtonsoft.Json;

namespace EzCadSync.Server.Utils;

public class ConfigurationLoader : BaseScript
{
    public static SyncConfiguration LoadConfiguration()
    {
        var fileContents = API.LoadResourceFile(API.GetCurrentResourceName(), "sync-config.json");
        return JsonConvert.DeserializeObject<SyncConfiguration>(fileContents);
    }
}