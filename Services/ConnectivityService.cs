using Microsoft.JSInterop;

namespace VolcanREG.Services;

public sealed class ConnectivityService
{
    private readonly IJSRuntime _jsRuntime;

    public ConnectivityService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<bool> IsOnlineAsync()
    {
        return await _jsRuntime.InvokeAsync<bool>("volcanConnectivity.isOnline");
    }
}
