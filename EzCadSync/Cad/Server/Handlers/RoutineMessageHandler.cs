using System;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace EzCadSync.Server.Handlers;

public class RoutineMessageHandler : BaseScript
{
    [Tick]
    public async Task HandleRoutineMessageAsync()
    {
        TriggerClientEvent("chat:addMessage", new
        {
            multiline = true,
            color = new[] {255, 255, 255},
            args = new[] {"System", MemoryStorage.Configuration.RoutineMessage}
        });

        await Delay((int) TimeSpan.FromMinutes(15).TotalMilliseconds);
    }
}