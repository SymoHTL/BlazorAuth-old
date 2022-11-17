# MudBlazor-Template

__This is my Defaut-Theme when i am doing a project with mudblazor and mysql.__

__And i added Authorization yay__

>Maybe you like it too.

<br>

### How to use and wtf does it do?

For the Theme cfg and logic you can visit my other project
[MudBlazor Theme](https://github.com/SymoHTL/MudBlazor-Template)

```c#
  Program.cs
  
  // this adds the basic built in services for Authentication for us
  // add this first
  builder.Services.AddAuthenticationCore();

  // the default blazor things
  builder.Services.AddRazorPages();
  builder.Services.AddServerSideBlazor();

  // then we can add the Options
  // this adds services we need
  builder.Services.AddOptions();

  // your databse connection if oyu work with Entityframework
  builder.Services.AddDbContextFactory<ModelDbContext>(
     options => options.UseMySql(
          builder.Configuration.GetConnectionString("DefaultConnection"),
          new MySqlServerVersion(new Version(8, 0, 26))
     ).UseLoggerFactory(new NullLoggerFactory())
     //.UseLoggerFactory(new NullLoggerFactory()) is to remove mysql query logging
  );

  // the services i created
  builder.Services.AddScoped<IThemeHandler, ThemeHandler>(); // manages the Theme of Mudblazor
  builder.Services.AddScoped<CircuitHandler, CircuitTracker>(); // retrives the Theme from the browser
  builder.Services.AddScoped<IUserRepository, UserRepository>(); // the UserRepository to perform CRUD operations on the User Table
  builder.Services.AddScoped<IRoleRepository, RoleRepository>(); // same as UserRepository but for our Roles
  builder.Services.AddScoped<IRoleClaimRepository, RoleClaimRepository>(); // the Repository for the n:m between users and roles


builder.Services.AddLogging(); // the default Logger

// the MudBlazor Configuration
builder.Services.AddMudServices(config => {
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 4000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Outlined;
});

builder.Services.AddHttpContextAccessor(); // this services enables us to access to HttpContext of our App

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>(); // we register our implementation of the AuthenticationStateProvider

builder.Services.AddScoped<UserService>(); // The UserService we use to login/register/logout
```

(not required for Auth)
```c#
CircuitTracker.cs

public class CircuitTracker : CircuitHandler { // we inherit from CircuitHandler because we need to ovveride som methods
    
    private readonly ProtectedLocalStorage _local;  // we inject ProtectedLocalStorage through dependency injection
    private readonly IThemeHandler _theme;    // the MudBlazor theme 
    private readonly ILogger<CircuitTracker> _logger; // Logger

    // Contructor (if you don't know what a constructor is you should not be here)
    public CircuitTracker(IThemeHandler theme, ProtectedLocalStorage local, ILogger<CircuitTracker> logger) {
        _theme = theme;
        _local = local;
        _logger = logger;
    }

    // we ovveride the OnConnectionUpAsync method provided from CircuitHandler
    // this is called when the blazor app recieves a connection
    // this is called and handled before anything else
    public override async Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken) {
        // our method to retrieve our custom vars from the browsers LocalStorage
        await GetStuff(cancellationToken);
    }

    private async Task GetStuff(CancellationToken ct) {
        // try catch to keep everything clean
        try {
            // we read the Theme from the browsers LocalStorage
            var theme = (await _local.GetAsync<Theme>("Theme")).Value;
            // if the theme is null it we just return
            if (theme is null) return;
            
            // otherwise we Update the theme using our IThemeHandler
            _theme.UpdateAll(theme);
        }
        // if a CryptographicException happens
        // this Exception occurs when you create a new Project but you already have values Stored in the browsers localstorage
        // so they can't be decrypted so it throws an error
        // we also MUST delete every value we need
        // so the theme for the theme
        // and the id for our Login
        catch(CryptographicException ex) {
            _logger.LogError(ex, "Failed to decrypt localstorage");
            await _local.DeleteAsync("Theme");
            await _local.DeleteAsync("Id");
        }
        catch (Exception e) {
            // if any other Exception happens we just log it
            _logger.LogError(e, "Failed to read from localstorage");
        }
    }
}
```
```c#
CustomAuthStateProvider.cs

public class CustomAuthStateProvider : AuthenticationStateProvider { // we inherit from AuthenticationStateProvider to override GetAuthenticationStateAsync()
    
    public User? CurrentUser { get; private set; }  // the current logged in User, is null when no user is logged in
    
    private readonly ProtectedLocalStorage _local;  // the LocalStorage Service so we can retrieve the login info from the browsers storage

    private readonly IUserRepository _userRepository; // the UserRepository to perform CRUD operations

    private readonly ILogger<CustomAuthStateProvider> _logger;  // logger

    // Contructor
    public CustomAuthStateProvider(IUserRepository userRepository, ProtectedLocalStorage local, ILogger<CustomAuthStateProvider> logger) {
        _userRepository = userRepository;
        _local = local;
        _logger = logger;
    }

    private static AuthenticationState Anonymous => new(new ClaimsPrincipal(new ClaimsIdentity())); // static autoProperty to get an Anonymous user (this represents an unauthorized user)
    
    // this method gets called everytime we want to authorize/Authenticate the user
    public override async Task<AuthenticationState> GetAuthenticationStateAsync() {
        // try catch to keep it clean
        try {
            // below defined method to retrieve the user from the database
            var user = await GetUserAsync();
            // if the user does not exist we return an Anonymous user
            if (user is null) return Anonymous;
            // if the user is successfully logged in we save it to CurrentUser so we can easily access it later
            CurrentUser = user;
            
            // we generate our claims for authorization
            var identity = new ClaimsIdentity(GenerateClaims(user), "GetStateType");

            // we return a new State based on our ClaimsPrincipal
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        // we can't call js methods at prerendering
        catch (InvalidOperationException) {
            return Anonymous; // most likely because of JavaScript interop
        }
        // other errors
        catch (Exception e) {
            _logger.LogError(e, "Error while getting authentication state");
            return Anonymous;
        }
    }
    
    // retrive the UserId from Browserstorage and the read the user from the database
    private async Task<User?> GetUserAsync() {
            // auth with id
            
            // get the Id we store upon Loggin in from the LocalStorage of the Browser
            var id = await _local.GetAsync<int>("id");

            // if the retrieve was a success and the id is not 0
            // we Authorize the user -- IUserRepository
            if (id is { Success: true, Value: not 0 }) return await _userRepository.AuthorizeAsync(id.Value);

            // here are some ideas which you cloud otherwise use for loggin in the user
            
            //    // auth with token if guid is not present
            //var token = await _local.GetAsync<string>("token");
            //if (token.Success && token.Value is not null)
            //    return await _userRepository.AuthorizeAsync(token.Value);

            //    // auth with email and password if token and guid are not present
            //var loginObject = await _local.GetAsync<LoginModel>("login");
            //if (loginObject.Success && loginObject.Value is not null)
            //    return await _userRepository.AuthorizeAsync(loginObject.Value);
            
            
            // if every logintry/loginmethod failed we just return null
        return null;
    }

    // we generate the claims for Auth/Authorization
    private static IEnumerable<Claim> GenerateClaims(User user) {
        // we add email and username, (i don't think we need them but why not)
        var claims = new List<Claim> {
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email)
        };
        // here we get the roles of the user and add them as Claims too
        // Authorization works with these RoleClaims
        claims.AddRange(user.PlainRoles.Select(role => new Claim(ClaimTypes.Role, role)));
        
        // we return them :)
        return claims;
    }

    // login the user
    public async Task Login(User user) {
        // update to currentuser to the provided user
        CurrentUser = user;
        // generate the claims
        var authState =
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(GenerateClaims(user), "LoginType")));
        NotifyAuthenticationStateChanged(Task.FromResult(authState));
        
        // add the id of the logged in user to the Browser's LocalStorage so the user can be automatically logged in when HE later accesses the app
        await _local.SetAsync("id", user.Id);
    }

    public async Task Logout() {
        //logout
        // set the CurrentUser to null
        CurrentUser = null;
        // delete the Id Key to prevent autologin
        await _local.DeleteAsync("id");
        //await _local.DeleteAsync("token");
        //await _local.DeleteAsync("login");
        // Notify the Auth handler to it is signaled to every component that it should refresh
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
    }
}
```

```c#
UserService.cs
public class UserService { // the Service to Manage Users

    public User? CurrentUser => _authenticationStateProvider.CurrentUser; // get the logged in user from our CustomAuthStateProvider
    
    private readonly CustomAuthStateProvider _authenticationStateProvider;  // CustomAuthStateProvider 

    private readonly IUserRepository _userRepository; // the repository to perform CRUD operations

    //Contructor
    public UserService(AuthenticationStateProvider authenticationStateProvider, IUserRepository userRepository) {
        _authenticationStateProvider = authenticationStateProvider
                                           as CustomAuthStateProvider ??
                                       throw new NullReferenceException(
                                           "I Guess you forgot to add the CustomAuthStateProvider to the Dependency Injection container");
        _userRepository = userRepository;
    }

    // register method
    public async Task RegisterAsync(User user, CancellationToken ct = default) {
        // check for unique email
        var userExists = await _userRepository.FindByEmailAsync(user.Email, ct);
        // if email exists we throw DuplicateEmailException
        if (userExists != null)
            throw new DuplicateEmailException();
        
        // Hash the password
        user.PasswordHash = User.HashPassword(user.LoginPassword);
        // create the user on the database
        await _userRepository.CreateAsync(user, ct);
    }

    // Log in the user
    public async Task LoginAsync(LoginModel loginModel, CancellationToken ct = default) {
        // we read from the database (Authorization is checked there)
        var user = await _userRepository.AuthorizeAsync(loginModel, ct);
        // if the user doesn't exist we throw LoginException
        if (user is null)
            throw new LoginException();
        
        // log in the user with our CustomAuthStateProvider
        await _authenticationStateProvider.Login(user);
    }

    public async Task LogoutAsync() {
        // Logout the user with CustomAuthStateProvider
        await _authenticationStateProvider.Logout();
    }
}
```


[CascadingValues](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/cascading-values-and-parameters?view=aspnetcore-7.0)
```razor
App.razor

<CascadingAuthenticationState> // this exposes the AuthenticationState for all our Pages/Components 

    <Router AppAssembly="@typeof(App).Assembly">
        <Found Context="routeData">
            <PageTitle>Symo - template</PageTitle>
            <AuthorizeRouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)"/> // the authorize route view to prevent anonymous user from visiting restriced pages
            <FocusOnNavigate RouteData="@routeData" Selector="h1"/>
        </Found>
        <NotFound>
            <PageTitle>Not found</PageTitle>
            <LayoutView Layout="@typeof(MainLayout)">
                <p role="alert">Sorry, there's nothing at this address.</p>
            </LayoutView>
        </NotFound>
    </Router>
</CascadingAuthenticationState>
```

### Yeah i am tired now. I mean this only took 30 min but idk

### maybe explaination for UserRepository, Login.razor, Reigster.razor, AccountMenu soon


<br><br>
Setup:
```
Just clone the repository and you should be good to go!
Or
Use this as a template and create a new repository!
```

[Licensing](/LICENSE)
