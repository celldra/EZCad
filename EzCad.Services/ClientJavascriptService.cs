using EzCad.Services.Interfaces;
using Microsoft.JSInterop;

namespace EzCad.Services;

public class ClientJavascriptService : IClientJavascriptService
{
    private readonly IJSRuntime _js;

    public ClientJavascriptService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task GoBackAsync()
    {
        await _js.InvokeVoidAsync("history.back");
    }
}