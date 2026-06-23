using Microsoft.JSInterop;
using VolcanREG.Models;

namespace VolcanREG.Services;

public sealed class ExcelExportService
{
    private readonly IJSRuntime _jsRuntime;

    public ExcelExportService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task ExportAsync(IEnumerable<LoadRecord> records)
    {
        await _jsRuntime.InvokeVoidAsync("volcanExcel.exportLoadRecords", records);
    }
}
