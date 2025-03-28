using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace GallagherCommands.Client
{
    public static class ExtensionMethods
    {
        private static readonly string[] DisabledPeds =
        {
            "AMBIENT_GANG_HILLBILLY", "AMBIENT_GANG_BALLAS", "AMBIENT_GANG_MEXICAN", "AMBIENT_GANG_FAMILY", "AMBIENT_GANG_MARABUNTE",
            "AMBIENT_GANG_SALVA", "GANG_1", "GANG_2", "GANG_9", "GANG_10", "FIREMAN", "MEDIC", "COP"
        };

        public static void SetPedRelationship(int relationshipLevel = 3)
        {
            var playerHashKey = (uint)API.GetHashKey("PLAYER");
            foreach (var ped in DisabledPeds)
            {
                API.SetRelationshipBetweenGroups(relationshipLevel, (uint) API.GetHashKey(ped), playerHashKey);
            }
        }
        
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