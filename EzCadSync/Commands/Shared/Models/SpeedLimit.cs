using Newtonsoft.Json;

namespace GallagherCommands.Client;

public class SpeedLimit
{
    [JsonConstructor]
    public SpeedLimit(string roadName, int limit)
    {
        RoadName = roadName;
        Limit = limit;
    }

    [JsonProperty("road_name")] public string RoadName { get; set; }
    [JsonProperty("limit")] public int Limit { get; set; }
}