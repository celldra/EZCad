using Newtonsoft.Json;

namespace GallagherCommands.Configuration.Models;

public class CoOrdinates
{
    [JsonProperty("x")] public double X { get; set; }
    [JsonProperty("y")] public double Y { get; set; }
    [JsonProperty("z")] public double Z { get; set; }
}