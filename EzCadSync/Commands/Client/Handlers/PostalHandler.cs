using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;

namespace GallagherCommands.Client.Handlers;

public class PostalHandler : BaseScript
{
    public static readonly List<Postal>? PostalList = PostalConfigurationManager.Load();

    public static string? NearestPostal;
    public static string? NearestPostalDistance;

    private static IEnumerable<float> YieldDistances(Vector2 playerPosition)
    {
        if (PostalList == null) yield break;
        
        foreach (var distance in PostalList.Select(postal => Vector2.Distance(playerPosition, postal.Position)))
        {
            yield return distance;
        }
    }

    public static Tuple<string, string> CalculatePostal(float x, float y)
    {
        var position = new Vector2(x, y);
        
        var distances = YieldDistances(position)
            .ToList();

        var nearestPostalIndex = distances.IndexOf(distances.Min());
        var nearestPostal = PostalList?[nearestPostalIndex].Code ?? "N/A";
        var nearestPostalDistance = distances.Min().ToString("N1");
        
        return Tuple.Create(nearestPostal, nearestPostalDistance);
    }

    [Tick]
    private async Task Calculate()
    {
        await Delay(500);
        
        var position = Game.PlayerPed.Position;

        var result = CalculatePostal(position.X, position.Y);
        NearestPostal = result.Item1;
        NearestPostalDistance = result.Item2;

        var message = new
        {
            type = "postal",
            postal = $"{NearestPostal} ({NearestPostalDistance}m)"
        };

        API.SendNuiMessage(JsonConvert.SerializeObject(message));
    }
}