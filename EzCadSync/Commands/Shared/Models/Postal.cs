using CitizenFX.Core;
using Newtonsoft.Json;

namespace GallagherCommands.Client;

public class Postal
{
    [JsonConstructor]
    public Postal(string? code, float x, float y)
    {
        Code = code;
        X = x;
        Y = y;
    }

    [JsonProperty("code")] public string? Code { get; }
    [JsonProperty("x")] public float X { get; }
    [JsonProperty("y")] public float Y { get; }
    [JsonIgnore] public Vector2 Position => new(X, Y);
}