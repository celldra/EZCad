using System;
using EzCadSync.Api.Responses;

namespace EzCadSync.Api.Exceptions;

public class BannedException : Exception
{
    public BannedException(string message, BanResponse response) : base(message)
    {
        BanResponse = response;
    }

    public BanResponse BanResponse { get; }
}