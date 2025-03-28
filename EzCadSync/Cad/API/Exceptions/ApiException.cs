using System;
using EzCadSync.Api.Responses;

namespace EzCadSync.Api.Exceptions;

public class ApiException : Exception
{
    public ApiException(string message, ErrorResponse? response = null) : base(message)
    {
        Response = response;
    }

    public ErrorResponse? Response { get; }
}