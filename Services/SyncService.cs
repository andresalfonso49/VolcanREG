using VolcanREG.Helpers;
using VolcanREG.Models;
using VolcanREG.Security;

namespace VolcanREG.Services;

public sealed class SyncService
{
    private readonly IndexedDbService _indexedDbService;
    private readonly FirebaseService _firebaseService;
    private readonly ConnectivityService _connectivityService;
    private readonly AuthService _authService;
    private bool _isRunning;

    public SyncService(
        IndexedDbService indexedDbService,
        FirebaseService firebaseService,
        ConnectivityService connectivityService,
        AuthService authService)
    {
        _indexedDbService = indexedDbService;
        _firebaseService = firebaseService;
        _connectivityService = connectivityService;
        _authService = authService;
    }

    public event Action? SyncStateChanged;

    public async Task<int> SynchronizeAsync()
    {
        if (_isRunning || !await _connectivityService.IsOnlineAsync())
        {
            return 0;
        }

        _isRunning = true;
        var synced = 0;

        try
        {
            var records = await _indexedDbService.GetRecordsForSyncAsync();
            if (records.Count > 0)
            {
                try
                {
                    await _authService.EnsureOnlineProfileIsActiveAsync();
                }
                catch (Exception ex)
                {
                    foreach (var record in records)
                    {
                        await _indexedDbService.MarkSyncStatusAsync(record.LocalId, SyncStatuses.Error, ex.Message);
                    }

                    return 0;
                }
            }

            foreach (var record in records)
            {
                await _indexedDbService.MarkSyncStatusAsync(record.LocalId, SyncStatuses.Syncing, null);

                try
                {
                    var remote = await _firebaseService.FindLoadRecordByClientRecordIdAsync(record.ClientRecordId);
                    if (remote is not null)
                    {
                        await _indexedDbService.MarkSyncedAsync(record.LocalId, remote.ServerRecordId);
                    }
                    else
                    {
                        record.SyncStatus = SyncStatuses.Synced;
                        record.ValidationStatus = ValidationStatuses.NotValidated;
                        record.SyncedAtUtc = DateTimeHelper.UtcNow();
                        var serverId = await _firebaseService.CreateLoadRecordAsync(record);
                        await _firebaseService.UpsertDriverAndVehicleAsync(record);
                        await _indexedDbService.MarkSyncedAsync(record.LocalId, serverId);
                    }

                    await _indexedDbService.SaveRecentDriverAndVehicleAsync(record);
                    await _firebaseService.CreateSyncLogAsync(new SyncLog
                    {
                        ClientRecordId = record.ClientRecordId,
                        DeviceId = record.DeviceId,
                        OperatorId = record.OperatorId,
                        AttemptedAtUtc = DateTimeHelper.UtcNow(),
                        Status = SyncStatuses.Synced
                    });
                    synced++;
                }
                catch (Exception ex)
                {
                    await _indexedDbService.MarkSyncStatusAsync(record.LocalId, SyncStatuses.Error, ex.Message);
                    try
                    {
                        await _firebaseService.CreateSyncLogAsync(new SyncLog
                        {
                            ClientRecordId = record.ClientRecordId,
                            DeviceId = record.DeviceId,
                            OperatorId = record.OperatorId,
                            AttemptedAtUtc = DateTimeHelper.UtcNow(),
                            Status = SyncStatuses.Error,
                            ErrorMessage = ex.Message
                        });
                    }
                    catch
                    {
                        // If Firestore is unavailable, keeping the local error is enough for the next retry.
                    }
                }
            }

            await _indexedDbService.SetLastSyncUtcAsync(DateTimeHelper.UtcNow());
            return synced;
        }
        finally
        {
            _isRunning = false;
            SyncStateChanged?.Invoke();
        }
    }
}
