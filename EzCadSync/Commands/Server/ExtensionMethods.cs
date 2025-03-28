using System;
using CitizenFX.Core;

namespace GallagherCommands.Server
{
    public static class ExtensionMethods
    {
        public static bool TryGetPlayer(this PlayerList players, int id, out Player? player)
        {
            player = null;
            try
            {
                var tmpPlayer = players[id];
                if (tmpPlayer is null) return false;

                player = tmpPlayer;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}