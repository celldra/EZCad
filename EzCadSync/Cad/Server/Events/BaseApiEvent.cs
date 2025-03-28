using System;
using CitizenFX.Core;
using EzCadSync.Api;

namespace EzCadSync.Server.Events;

public class BaseApiEvent : BaseScript, IDisposable
{
    protected readonly ApiService Api = new();

    public void Dispose()
    {
        Api?.Dispose();
    }
}