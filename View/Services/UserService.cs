using Domain.Exceptions;
using Domain.Exceptions.Token;

namespace View.Services;

public class UserService {
    private readonly CustomAuthStateProvider _authenticationStateProvider;
    private readonly ITokenRepository _tokenRepo;

    private readonly IUserRepository _userRepo;

    public UserService(AuthenticationStateProvider authenticationStateProvider, IUserRepository userRepo,
        ITokenRepository tokenRepo) {
        _authenticationStateProvider = authenticationStateProvider
                                           as CustomAuthStateProvider ??
                                       throw new NullReferenceException(
                                           "I Guess you forgot to add the CustomAuthStateProvider to the Dependency Injection container");
        _userRepo = userRepo;
        _tokenRepo = tokenRepo;
    }

    public User? CurrentUser => _authenticationStateProvider.CurrentUser;

    public Task<AuthenticationState> GetAuthenticationStateAsync() =>
        _authenticationStateProvider.GetAuthenticationStateAsync();

    public async Task<bool> IsAuthenticated() {
        var identity = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return identity.User.Identity is { IsAuthenticated: true };
    }

    public async Task<bool> HasRole(string role) {
        var identity = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return identity.User.IsInRole(role);
    }

    public async Task RegisterAsync(User user, CancellationToken ct = default) {
        // check for unique stuff
        var userExists = await _userRepo.ReadAuthGraphAsync(user.Email, ct);
        if (userExists != null)
            throw new DuplicateEmailException();

        user.PasswordHash = User.HashPassword(user.LoginPassword);
        await _userRepo.CreateAsync(user, ct);
    }

    public async Task LoginAsync(LoginModel loginModel, CancellationToken ct = default) {
        var user = await _userRepo.AuthorizeAsync(loginModel, ct);
        if (user is null)
            throw new LoginException();
        var token = await _tokenRepo.GenerateTokenAsync(user, loginModel.DeviceName, ct);
        if (token is null)
            throw new GenerateTokenException();

        await _authenticationStateProvider.Login(user, token.Identifier);
    }

    public async Task LogoutAsync() {
        await _authenticationStateProvider.Logout();
    }
}