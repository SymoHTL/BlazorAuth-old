﻿namespace View.Services;

public class UserService {
    private readonly CustomAuthStateProvider _authenticationStateProvider;
    private readonly ITokenRepository _tokenRepository;

    private readonly IUserRepository _userRepository;

    public UserService(AuthenticationStateProvider authenticationStateProvider, IUserRepository userRepository,
        ITokenRepository tokenRepository) {
        _authenticationStateProvider = authenticationStateProvider
                                           as CustomAuthStateProvider ??
                                       throw new NullReferenceException(
                                           "I Guess you forgot to add the CustomAuthStateProvider to the Dependency Injection container");
        _userRepository = userRepository;
        _tokenRepository = tokenRepository;
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
        var token = await _tokenRepository.RequestTokenAsync(user, ct);

        await _authenticationStateProvider.Login(user, token);
    }

    public async Task LogoutAsync() {
        await _authenticationStateProvider.Logout();
    }
}