using System.Collections.Generic;
using System.Linq;
using EzCadSync.Api.Models;

namespace EzCadSync.Api.Exceptions;

public class BadRequestException : ApiException
{
    public IReadOnlyCollection<ValidationError> Errors { get; }

    public BadRequestException(string message, IReadOnlyCollection<ValidationError> errors) : base(errors.Count == 0
        ? message
        : $"({errors.FirstOrDefault()?.Field}) {errors.FirstOrDefault()?.Message}")
    {
        Errors = errors;
    }
}