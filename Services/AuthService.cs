using VolcanREG.Models;

namespace VolcanREG.Services;

public sealed class AuthService
{
    private static readonly TimeSpan OfflineProfileValidity = TimeSpan.FromDays(7);
    private readonly FirebaseService _firebaseService;
    private readonly IndexedDbService _indexedDbService;
    private readonly ConnectivityService _connectivityService;

    public AuthService(
        FirebaseService firebaseService,
        IndexedDbService indexedDbService,
        ConnectivityService connectivityService)
    {
        _firebaseService = firebaseService;
        _indexedDbService = indexedDbService;
        _connectivityService = connectivityService;
    }

    public FirebaseUserSession? Session { get; private set; }
    public UserProfile? Profile { get; private set; }
    public bool IsAuthenticated => Session is not null && Profile is not null;

    public event Action? AuthStateChanged;

    public async Task InitializeAsync()
    {
        if (await _connectivityService.IsOnlineAsync())
        {
            try
            {
                Session = await _firebaseService.GetCurrentUserAsync();
                Profile = Session is null ? null : await _firebaseService.GetUserProfileAsync(Session.Uid);

                if (Profile is not null)
                {
                    if (!Profile.IsActive)
                    {
                        await LogoutAsync();
                        return;
                    }

                    await CacheProfileAsync(Profile);
                    AuthStateChanged?.Invoke();
                    return;
                }
            }
            catch
            {
                // Fall back to the cached profile below. This keeps the app usable in weak coverage.
            }
        }

        await InitializeFromCachedProfileAsync();
        AuthStateChanged?.Invoke();
    }

    public async Task LoginAsync(string email, string password)
    {
        Session = await _firebaseService.SignInAsync(email, password);
        if (Session is null)
        {
            throw new InvalidOperationException("No se pudo iniciar sesion.");
        }

        Profile = await _firebaseService.GetUserProfileAsync(Session.Uid);
        if (Profile is null)
        {
            await LogoutAsync();
            throw new InvalidOperationException("El usuario no tiene perfil en Firestore.");
        }

        if (!Profile.IsActive)
        {
            await LogoutAsync();
            throw new InvalidOperationException("El usuario esta inactivo.");
        }

        await CacheProfileAsync(Profile);
        AuthStateChanged?.Invoke();
    }

    public async Task EnsureOnlineProfileIsActiveAsync()
    {
        var currentSession = await _firebaseService.GetCurrentUserAsync();
        if (currentSession is null)
        {
            throw new InvalidOperationException("Debes iniciar sesion con internet antes de sincronizar.");
        }

        var profile = await _firebaseService.GetUserProfileAsync(currentSession.Uid);
        if (profile is null)
        {
            throw new InvalidOperationException("El usuario no tiene perfil en Firestore.");
        }

        if (!profile.IsActive)
        {
            await LogoutAsync();
            throw new InvalidOperationException("Usuario inactivo. Contacte al administrador.");
        }

        Session = currentSession;
        Profile = profile;
        await CacheProfileAsync(profile);
        AuthStateChanged?.Invoke();
    }

    public async Task LogoutAsync()
    {
        if (await _connectivityService.IsOnlineAsync())
        {
            try
            {
                await _firebaseService.SignOutAsync();
            }
            catch
            {
                // Local logout must still clear offline access.
            }
        }

        await _indexedDbService.ClearCachedUserProfileAsync();
        Session = null;
        Profile = null;
        AuthStateChanged?.Invoke();
    }

    private async Task CacheProfileAsync(UserProfile profile)
    {
        await _indexedDbService.SaveCachedUserProfileAsync(profile);
        await _indexedDbService.SetLastProfileCheckUtcAsync(DateTime.UtcNow);
    }

    private async Task InitializeFromCachedProfileAsync()
    {
        var cachedProfile = await _indexedDbService.GetCachedUserProfileAsync();
        var lastCheckUtc = await _indexedDbService.GetLastProfileCheckUtcAsync();

        if (cachedProfile is null || lastCheckUtc is null)
        {
            Session = null;
            Profile = null;
            return;
        }

        var age = DateTime.UtcNow - DateTime.SpecifyKind(lastCheckUtc.Value, DateTimeKind.Utc);
        if (age > OfflineProfileValidity)
        {
            Session = null;
            Profile = null;
            return;
        }

        Profile = cachedProfile;
        Session = new FirebaseUserSession
        {
            Uid = cachedProfile.Uid,
            Email = cachedProfile.Email,
            DisplayName = cachedProfile.DisplayName
        };
    }
}
