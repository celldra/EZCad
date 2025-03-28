using CitizenFX.Core;
using CitizenFX.Core.Native;
using EzCadSync.Api.Models;
using Newtonsoft.Json;

namespace EzCadSync.Api.Utils;

public class ConfigurationLoader : BaseScript
{
    public static SyncConfiguration LoadConfiguration()
    {
        var fileContents = API.LoadResourceFile(API.GetCurrentResourceName(), "sync-config.json");
        return JsonConvert.DeserializeObject<SyncConfiguration>(fileContents);
    }
}