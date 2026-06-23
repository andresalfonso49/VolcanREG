using Microsoft.JSInterop;
using VolcanREG.Models;

namespace VolcanREG.Services;

public sealed class FirebaseService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly FirebaseOptions _options;
    private bool _initialized;

    public FirebaseService(IJSRuntime jsRuntime, FirebaseOptions options)
    {
        _jsRuntime = jsRuntime;
        _options = options;
    }

    private bool HasConfig => !string.IsNullOrWhiteSpace(_options.ApiKey)
        && !string.IsNullOrWhiteSpace(_options.ProjectId);

    public async Task InitializeAsync()
    {
        if (_initialized)
        {
            return;
        }

        await _jsRuntime.InvokeVoidAsync("volcanFirebase.initialize", _options);
        _initialized = true;
    }

    public async Task<FirebaseUserSession?> SignInAsync(string email, string password)
    {
        await InitializeAsync();
        return await _jsRuntime.InvokeAsync<FirebaseUserSession?>("volcanFirebase.signIn", email, password);
    }

    public async Task SignOutAsync()
    {
        await InitializeAsync();
        await _jsRuntime.InvokeVoidAsync("volcanFirebase.signOut");
    }

    public async Task<FirebaseUserSession?> GetCurrentUserAsync()
    {
        if (!HasConfig)
        {
            return null;
        }

        await InitializeAsync();
        return await _jsRuntime.InvokeAsync<FirebaseUserSession?>("volcanFirebase.getCurrentUser");
    }

    public async Task<UserProfile?> GetUserProfileAsync(string uid)
    {
        await InitializeAsync();
        return await _jsRuntime.InvokeAsync<UserProfile?>("volcanFirebase.getUserProfile", uid);
    }

    public async Task<IReadOnlyList<UserProfile>> GetUsersAsync()
    {
        await InitializeAsync();
        return await _jsRuntime.InvokeAsync<UserProfile[]>("volcanFirebase.getUsers");
    }

    public async Task<IReadOnlyList<Driver>> GetDriversAsync()
    {
        await InitializeAsync();
        return await _jsRuntime.InvokeAsync<Driver[]>("volcanFirebase.getDrivers");
    }

    public async Task<IReadOnlyList<Vehicle>> GetVehiclesAsync()
    {
        await InitializeAsync();
        return await _jsRuntime.InvokeAsync<Vehicle[]>("volcanFirebase.getVehicles");
    }

    public async Task<UserProfile> CreateUserAsync(string email, string password, string displayName, string role, bool isActive)
    {
        await InitializeAsync();
        return await _jsRuntime.InvokeAsync<UserProfile>("volcanFirebase.createUser", email, password, displayName, role, isActive);
    }

    public async Task UpdateUserProfileAsync(UserProfile profile)
    {
        await InitializeAsync();
        await _jsRuntime.InvokeVoidAsync("volcanFirebase.updateUserProfile", profile);
    }

    public async Task<LoadRecord?> FindLoadRecordByClientRecordIdAsync(string clientRecordId)
    {
        await InitializeAsync();
        return await _jsRuntime.InvokeAsync<LoadRecord?>("volcanFirebase.findLoadRecordByClientRecordId", clientRecordId);
    }

    public async Task<string> CreateLoadRecordAsync(LoadRecord record)
    {
        await InitializeAsync();
        return await _jsRuntime.InvokeAsync<string>("volcanFirebase.createLoadRecord", record);
    }

    public async Task<IReadOnlyList<LoadRecord>> GetLoadRecordsAsync()
    {
        await InitializeAsync();
        return await _jsRuntime.InvokeAsync<LoadRecord[]>("volcanFirebase.getLoadRecords");
    }

    public async Task<LoadRecord?> GetLoadRecordAsync(string id)
    {
        await InitializeAsync();
        return await _jsRuntime.InvokeAsync<LoadRecord?>("volcanFirebase.getLoadRecord", id);
    }

    public async Task UpdateLoadRecordAsync(LoadRecord record)
    {
        await InitializeAsync();
        await _jsRuntime.InvokeVoidAsync("volcanFirebase.updateLoadRecord", record);
    }

    public async Task CreateEditLogsAsync(IEnumerable<EditLog> logs)
    {
        await InitializeAsync();
        await _jsRuntime.InvokeVoidAsync("volcanFirebase.createEditLogs", logs);
    }

    public async Task<IReadOnlyList<EditLog>> GetEditLogsAsync(string loadRecordId)
    {
        await InitializeAsync();
        return await _jsRuntime.InvokeAsync<EditLog[]>("volcanFirebase.getEditLogs", loadRecordId);
    }

    public async Task CreateValidationLogAsync(ValidationLog log)
    {
        await InitializeAsync();
        await _jsRuntime.InvokeVoidAsync("volcanFirebase.createValidationLog", log);
    }

    public async Task<IReadOnlyList<ValidationLog>> GetValidationLogsAsync(string loadRecordId)
    {
        await InitializeAsync();
        return await _jsRuntime.InvokeAsync<ValidationLog[]>("volcanFirebase.getValidationLogs", loadRecordId);
    }

    public async Task UpsertDriverAndVehicleAsync(LoadRecord record)
    {
        await InitializeAsync();
        await _jsRuntime.InvokeVoidAsync("volcanFirebase.upsertDriverAndVehicle", record);
    }

    public async Task CreateSyncLogAsync(SyncLog log)
    {
        await InitializeAsync();
        await _jsRuntime.InvokeVoidAsync("volcanFirebase.createSyncLog", log);
    }

    public async Task ResetOperationalDatabaseAsync()
    {
        await InitializeAsync();
        await _jsRuntime.InvokeVoidAsync("volcanFirebase.resetOperationalDatabase");
    }
}
