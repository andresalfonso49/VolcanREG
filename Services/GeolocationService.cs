using Microsoft.JSInterop;

namespace VolcanREG.Services;

public sealed class GeolocationService
{
    private readonly IJSRuntime _jsRuntime;

    public GeolocationService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<(double? Latitude, double? Longitude)> TryGetLocationAsync()
    {
        try
        {
            var result = await _jsRuntime.InvokeAsync<GeoLocationResult?>("volcanGeolocation.tryGetCurrentPosition");
            return (result?.Latitude, result?.Longitude);
        }
        catch
        {
            return (null, null);
        }
    }

    private sealed class GeoLocationResult
    {
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
