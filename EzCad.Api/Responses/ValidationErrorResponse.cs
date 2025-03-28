using System.Text.Json.Serialization;
using EzCad.Api.Models;
using EzCad.Shared.Responses;

namespace EzCad.Api.Responses;

public class ValidationErrorResponse : ErrorResponse
{
    [JsonPropertyName("errors")] public List<ValidationError> Errors { get; set; }
}