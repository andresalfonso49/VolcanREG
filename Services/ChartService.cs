using Microsoft.JSInterop;

namespace VolcanREG.Services;

public sealed class ChartService
{
    private readonly IJSRuntime _jsRuntime;

    public ChartService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task RenderBarAsync(string canvasId, string title, IReadOnlyDictionary<string, decimal> data)
    {
        await _jsRuntime.InvokeVoidAsync("volcanCharts.renderBar", canvasId, title, data);
    }

    public async Task RenderBarAsync(string canvasId, string title, IReadOnlyDictionary<string, int> data)
    {
        await _jsRuntime.InvokeVoidAsync("volcanCharts.renderBar", canvasId, title, data);
    }

    public async Task RenderDoughnutAsync(string canvasId, string title, IReadOnlyDictionary<string, int> data)
    {
        await _jsRuntime.InvokeVoidAsync("volcanCharts.renderDoughnut", canvasId, title, data);
    }

    public async Task RenderLineAsync(string canvasId, string title, IReadOnlyDictionary<string, decimal> data)
    {
        await _jsRuntime.InvokeVoidAsync("volcanCharts.renderLine", canvasId, title, data);
    }
}
