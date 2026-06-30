using Microsoft.JSInterop;
using VolcanREG.Models;
using VolcanREG.Security;

namespace VolcanREG.Services;

public sealed class IndexedDbService
{
    private readonly IJSRuntime _jsRuntime;
    private bool _initialized;

    public IndexedDbService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeAsync()
    {
        if (_initialized)
        {
            return;
        }

        await _jsRuntime.InvokeVoidAsync("volcanIndexedDb.initialize");
        _initialized = true;
    }

    public async Task<string> GetDeviceIdAsync()
    {
        await InitializeAsync();
        return await _jsRuntime.InvokeAsync<string>("volcanIndexedDb.getDeviceId");
    }

    public async Task SaveLoadRecordAsync(LoadRecord record)
    {
        await InitializeAsync();
        await _jsRuntime.InvokeVoidAsync("volcanIndexedDb.saveLoadRecord", record);
    }

    public async Task UpdateLoadRecordAsync(LoadRecord record)
    {
        await InitializeAsync();
        await _jsRuntime.InvokeVoidAsync("volcanIndexedDb.saveLoadRecord", record);
    }

    public async Task<IReadOnlyList<LoadRecord>> GetAllLoadRecordsAsync()
    {
        await InitializeAsync();
        return await _jsRuntime.InvokeAsync<LoadRecord[]>("volcanIndexedDb.getAllLoadRecords");
    }

    public async Task<IReadOnlyList<LoadRecord>> GetOperatorRecordsForDateAsync(string operatorId, DateTime date)
    {
        var records = await GetAllLoadRecordsAsync();
        return records.Where(x => x.OperatorId == operatorId && x.LoadedAtLocal.Date == date.Date)
            .OrderByDescending(x => x.LoadedAtLocal)
            .ToArray();
    }

    public async Task<IReadOnlyList<LoadRecord>> GetRecordsForSyncAsync()
    {
        var records = await GetAllLoadRecordsAsync();
        return records
            .Where(x => x.SyncStatus is SyncStatuses.Pending or SyncStatuses.Error)
            .OrderBy(x => x.CreatedAtUtc)
            .ToArray();
    }

    public async Task<int> GetPendingCountAsync()
    {
        var records = await GetRecordsForSyncAsync();
        return records.Count;
    }

    public async Task MarkSyncedAsync(string localId, string? serverRecordId)
    {
        await InitializeAsync();
        await _jsRuntime.InvokeVoidAsync("volcanIndexedDb.markSynced", localId, serverRecordId);
    }

    public async Task MarkSyncStatusAsync(string localId, string syncStatus, string? error)
    {
        await InitializeAsync();
        await _jsRuntime.InvokeVoidAsync("volcanIndexedDb.markSyncStatus", localId, syncStatus, error);
    }

    public async Task SaveRecentDriverAndVehicleAsync(LoadRecord record)
    {
        await InitializeAsync();
        await _jsRuntime.InvokeVoidAsync("volcanIndexedDb.saveRecentDriverAndVehicle", record);
    }

    public async Task<IReadOnlyList<Driver>> GetDriversAsync()
    {
        await InitializeAsync();
        return await _jsRuntime.InvokeAsync<Driver[]>("volcanIndexedDb.getDrivers");
    }

    public async Task<IReadOnlyList<Vehicle>> GetVehiclesAsync()
    {
        await InitializeAsync();
        return await _jsRuntime.InvokeAsync<Vehicle[]>("volcanIndexedDb.getVehicles");
    }

    public async Task<DateTime?> GetLastSyncUtcAsync()
    {
        await InitializeAsync();
        return await _jsRuntime.InvokeAsync<DateTime?>("volcanIndexedDb.getLastSyncUtc");
    }

    public async Task SetLastSyncUtcAsync(DateTime value)
    {
        await InitializeAsync();
        await _jsRuntime.InvokeVoidAsync("volcanIndexedDb.setLastSyncUtc", value);
    }

    public async Task<UserProfile?> GetCachedUserProfileAsync()
    {
        await InitializeAsync();
        return await _jsRuntime.InvokeAsync<UserProfile?>("volcanIndexedDb.getCachedUserProfile");
    }

    public async Task SaveCachedUserProfileAsync(UserProfile profile)
    {
        await InitializeAsync();
        await _jsRuntime.InvokeVoidAsync("volcanIndexedDb.setCachedUserProfile", profile);
    }

    public async Task ClearCachedUserProfileAsync()
    {
        await InitializeAsync();
        await _jsRuntime.InvokeVoidAsync("volcanIndexedDb.clearCachedUserProfile");
    }

    public async Task<DateTime?> GetLastProfileCheckUtcAsync()
    {
        await InitializeAsync();
        return await _jsRuntime.InvokeAsync<DateTime?>("volcanIndexedDb.getLastProfileCheckUtc");
    }

    public async Task SetLastProfileCheckUtcAsync(DateTime value)
    {
        await InitializeAsync();
        await _jsRuntime.InvokeVoidAsync("volcanIndexedDb.setLastProfileCheckUtc", value);
    }
}
