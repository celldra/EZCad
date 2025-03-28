using GallagherCommands.Shared.Models;

namespace GallagherCommands.Shared;

public static class MemoryStorage
{
    public static bool IsPeaceTimeEnabled = false;
    public static bool IsDisablingGamertags = false;
    public static bool IsShowingGamertags = false;
    public static PriorityStatus PriorityStatus = PriorityStatus.None;
    public static int PriorityTime = 0;

    public static string DefaultAop =
        ConfigurationManager.Load()?.DefaultAop ?? "Sandy Shores";
}