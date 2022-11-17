namespace View.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider {
    
    public User? CurrentUser { get; private set; }
    
    private readonly ProtectedLocalStorage _local;

    private readonly IUserRepository _userRepository;

    private readonly ILogger<CustomAuthStateProvider> _logger;

    public CustomAuthStateProvider(IUserRepository userRepository, ProtectedLocalStorage local, ILogger<CustomAuthStateProvider> logger) {
        _userRepository = userRepository;
        _local = local;
        _logger = logger;
    }

    private static AuthenticationState Anonymous => new(new ClaimsPrincipal(new ClaimsIdentity()));
    

    public override async Task<AuthenticationState> GetAuthenticationStateAsync() {
        try {
            var user = await GetUserAsync();
            if (user is null) return Anonymous;
            CurrentUser = user;

            var identity = new ClaimsIdentity(GenerateClaims(user), "GetStateType");


            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch (InvalidOperationException) {
            return Anonymous; // most likely because of JavaScript interop
        }
        catch (Exception e) {
            _logger.LogError(e, "Error while getting authentication state");
            return Anonymous;
        }
    }

    private async Task<User?> GetUserAsync() {
            // auth with id
            var id = await _local.GetAsync<int>("id");

            if (id is { Success: true, Value: not 0 }) return await _userRepository.AuthorizeAsync(id.Value);

            //    // auth with token if guid is not present
            //var token = await _local.GetAsync<string>("token");
            //if (token.Success && token.Value is not null)
            //    return await _userRepository.AuthorizeAsync(token.Value);

            //    // auth with email and password if token and guid are not present
            //var loginObject = await _local.GetAsync<LoginModel>("login");
            //if (loginObject.Success && loginObject.Value is not null)
            //    return await _userRepository.AuthorizeAsync(loginObject.Value);
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

    public async Task Login(User user) {
        CurrentUser = user;
        var authState =
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(GenerateClaims(user), "LoginType")));
        NotifyAuthenticationStateChanged(Task.FromResult(authState));
        await _local.SetAsync("id", user.Id);
    }

    public async Task Logout() {
        CurrentUser = null;
        await _local.DeleteAsync("id");
        //await _local.DeleteAsync("token");
        //await _local.DeleteAsync("login");
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
    }
}