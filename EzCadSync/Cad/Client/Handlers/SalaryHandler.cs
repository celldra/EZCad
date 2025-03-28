using System;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace EzCadSync.Client.Handlers;

public class SalaryHandler : BaseScript
{
    [Tick]
    public async Task HandleAsync()
    {
        TriggerServerEvent("EZCad:GetSalary");

        await Delay((int) TimeSpan.FromMinutes(30).TotalMilliseconds);
    }
}