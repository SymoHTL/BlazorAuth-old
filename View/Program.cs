using Domain.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// authorization
builder.Services.AddAuthenticationCore();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddOptions();

builder.Services.AddDbContextFactory<ModelDbContext>(
    options => options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 26))
    ).UseLoggerFactory(new NullLoggerFactory())
    //.UseLoggerFactory(new NullLoggerFactory()) is to remove mysql query logging
);

builder.Services.AddScoped<IThemeHandler, ThemeHandler>();
builder.Services.AddScoped<CircuitHandler, CircuitTracker>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleClaimRepository, RoleClaimRepository>();


builder.Services.AddScoped<DeviceService>();


builder.Services.AddLogging();

builder.Services.AddHttpContextAccessor();
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


builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

builder.Services.AddScoped<UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();


app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();