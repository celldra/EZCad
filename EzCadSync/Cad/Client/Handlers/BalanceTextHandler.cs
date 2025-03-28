using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace EzCadSync.Client.Handlers;

public class BalanceTextHandler : BaseScript
{
    private static readonly string[] Days = 
    {
        "SUNDAY", "MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY", "SATURDAY"
    };
    
    [Tick]
    public Task HandleAsync()
    {
        var hours = GetClockHours();
        var minutes = GetClockMinutes();
        
        BeginScaleformMovieMethodOnFrontendHeader("SET_HEADING_DETAILS");
        ScaleformMovieMethodAddParamTextureNameString(Game.Player.Name);
        ScaleformMovieMethodAddParamTextureNameString($"{Days[GetClockDayOfWeek()]} {hours:00}:{minutes:00}");
        ScaleformMovieMethodAddParamTextureNameString($"{MemoryStorage.Balance:C}");
        ScaleformMovieMethodAddParamBool(false);
        EndScaleformMovieMethod();
        
        return Task.FromResult(true);
    }
}