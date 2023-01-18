using Domain.Exceptions.TokenExceptions;

namespace View.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider {
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

    private readonly ProtectedLocalStorage _local;

    private readonly ILogger<CustomAuthStateProvider> _logger;

    private readonly IUserRepository _userRepository;

    // cache the current AuthenticationState to avoid unnecessary calls to the server
    // the state is cached for 5 minutes
    private AuthenticationState? _cachedState;
    private DateTime _cachedStateTime;

    public CustomAuthStateProvider(IUserRepository userRepository, ProtectedLocalStorage local,
        ILogger<CustomAuthStateProvider> logger) {
        _userRepository = userRepository;
        _local = local;
        _logger = logger;
    }

    public User? CurrentUser { get; private set; }

    private static AuthenticationState Anonymous => new(new ClaimsPrincipal(new ClaimsIdentity()));

    private void SetCachedState(AuthenticationState state) {
        _cachedState = state;
        _cachedStateTime = DateTime.Now;
    }

    private void ClearCache() {
        _cachedState = null;
        _cachedStateTime = DateTime.MaxValue;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync() {
        try {
            // caching
            if (_cachedState is not null && DateTime.Now - _cachedStateTime < _cacheDuration)
                return _cachedState;
            ClearCache();

            var user = await GetUserAsync();
            if (user is null) return Anonymous;
            CurrentUser = user;

            var identity = new ClaimsIdentity(GenerateClaims(user), "GetStateType");
            var authState = new AuthenticationState(new ClaimsPrincipal(identity));

            SetCachedState(authState);
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
            return authState;
        }
        catch (CryptographicException) {
            await _local.DeleteAsync("token");
            return Anonymous; // token encryption has changed
        }
        catch (InvalidOperationException) {
            return Anonymous; // most likely because of JavaScript interop
        }
        catch (TokenException) {
            await _local.DeleteAsync("token");
            return Anonymous; // token is invalid or expired or has been tampered with or deleted
        }
        catch (Exception e) {
            _logger.LogError(e, "Error while getting authentication state");
            return Anonymous;
        }
    }

    private async Task<User?> GetUserAsync() {
        // auth with id
        var token = await _local.GetAsync<string>("token");

        if (token is { Success: true, Value: not null }) return await _userRepository.AuthorizeAsync(token.Value);

        return null;
    }

    private static IEnumerable<Claim> GenerateClaims(User user) {
        var claims = new List<Claim> {
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email)
        };
        claims.AddRange(user.PlainRoles.Select(role => new Claim(ClaimTypes.Role, role)));
        return claims;
    }

    public async Task Login(User user, string token) {
        ClearCache();
        CurrentUser = user;
        var authState =
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(GenerateClaims(user), "LoginType")));

        SetCachedState(authState);

        NotifyAuthenticationStateChanged(Task.FromResult(authState));
        await _local.SetAsync("token", token);
    }

    public async Task Logout() {
        CurrentUser = null;
        ClearCache();
        await _local.DeleteAsync("token");
        //await _local.DeleteAsync("token");
        //await _local.DeleteAsync("login");
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
    }
}