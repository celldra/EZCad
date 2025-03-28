using System.Threading.Tasks;

namespace GallagherCommands.Shared.Timers
{
    public interface ITimer
    {
        Task RunTimer();
    }
}