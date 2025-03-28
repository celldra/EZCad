using CitizenFX.Core;
using GallagherCommands.Server.Commands;

namespace GallagherCommands.Server;

public class ServerMain : BaseScript
{
    public ServerMain()
    {
        Debug.WriteLine("Cmon you know, let's initialize every server command");
        
        _ = new AnnounceCommand();
        _ = new PeaceTimeCommand();
        _ = new RespawnCommand();
        _ = new ReviveCommand();
        _ = new DeleteAllVehiclesCommand();
        _ = new ShowGamertagsCommand();
        
        Debug.WriteLine("Done");
    }
}