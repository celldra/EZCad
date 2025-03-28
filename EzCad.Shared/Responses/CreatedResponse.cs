using System.Text.Json.Serialization;

namespace EzCad.Shared.Responses;

/// <summary>
///     Represents a response served when an entity is created
/// </summary>
public class CreatedResponse<TEntity> : BaseResponse
{
    public CreatedResponse(TEntity entity)
    {
        Entity = entity;
    }

    /// <summary>
    ///     The new entity created
    /// </summary>
    [JsonPropertyName("entity")]
    public TEntity Entity { get; set; }
}