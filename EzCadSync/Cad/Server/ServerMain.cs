using CitizenFX.Core;
using EzCadSync.Server.Commands;
using EzCadSync.Server.Utils;
using Newtonsoft.Json;

namespace EzCadSync.Server;

public class ServerMain : BaseScript
{
    public ServerMain()
    {
        Debug.WriteLine("Loading sync configuration");
        MemoryStorage.Configuration = ConfigurationLoader.LoadConfiguration();

        Debug.WriteLine("Sending configuration to clients");
        TriggerClientEvent("EZCad:UpdateConfiguration", JsonConvert.SerializeObject(MemoryStorage.Configuration));

        _ = new LinkCadCommand();
    }
}