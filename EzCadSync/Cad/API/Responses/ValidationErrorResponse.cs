using System.Collections.Generic;
using EzCadSync.Api.Models;
using Newtonsoft.Json;

namespace EzCadSync.Api.Responses;

public class ValidationErrorResponse : BaseResponse
{
    [JsonProperty("errors")] public List<ValidationError> Errors { get; set; }
}