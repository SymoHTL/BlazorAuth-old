using Domain.Exceptions;

namespace View.Services;

public class UserService {

    public User? CurrentUser => _authenticationStateProvider.CurrentUser;
    
    public Task<AuthenticationState> GetAuthenticationStateAsync() => _authenticationStateProvider.GetAuthenticationStateAsync();

    private readonly CustomAuthStateProvider _authenticationStateProvider;

    private readonly IUserRepository _userRepository;

    public UserService(AuthenticationStateProvider authenticationStateProvider, IUserRepository userRepository) {
        _authenticationStateProvider = authenticationStateProvider
                                           as CustomAuthStateProvider ??
                                       throw new NullReferenceException(
                                           "I Guess you forgot to add the CustomAuthStateProvider to the Dependency Injection container");
        _userRepository = userRepository;
    }

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
        var userExists = await _userRepository.FindByEmailAsync(user.Email, ct);
        if (userExists != null)
            throw new DuplicateEmailException();

        user.PasswordHash = User.HashPassword(user.LoginPassword);
        await _userRepository.CreateAsync(user, ct);
    }

    public async Task LoginAsync(LoginModel loginModel, CancellationToken ct = default) {
        var user = await _userRepository.AuthorizeAsync(loginModel, ct);
        if (user is null)
            throw new LoginException();

        await _authenticationStateProvider.Login(user);
    }

    public async Task LogoutAsync() {
        await _authenticationStateProvider.Logout();
    }
}