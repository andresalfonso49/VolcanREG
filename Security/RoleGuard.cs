using Microsoft.AspNetCore.Components;
using VolcanREG.Services;

namespace VolcanREG.Security;

public sealed class RoleGuard
{
    private readonly AuthService _authService;
    private readonly NavigationManager _navigationManager;

    public RoleGuard(AuthService authService, NavigationManager navigationManager)
    {
        _authService = authService;
        _navigationManager = navigationManager;
    }

    public async Task<bool> RequireRoleAsync(string role)
    {
        await _authService.InitializeAsync();

        if (_authService.Profile is null)
        {
            _navigationManager.NavigateTo("/login");
            return false;
        }

        if (_authService.Profile.Role != role)
        {
            _navigationManager.NavigateTo(_authService.Profile.IsAdmin ? "/admin" : "/operator");
            return false;
        }

        return true;
    }
}
