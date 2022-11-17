using Model.Entities.Theme;

namespace View;

public class CircuitTracker : CircuitHandler {
    
    private readonly ProtectedLocalStorage _local;
    private readonly IThemeHandler _theme;
    private readonly ILogger<CircuitTracker> _logger;

    public CircuitTracker(IThemeHandler theme, ProtectedLocalStorage local, ILogger<CircuitTracker> logger) {
        _theme = theme;
        _local = local;
        _logger = logger;
    }

    public override async Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken) {
        await GetStuff(cancellationToken);
    }

    private async Task GetStuff(CancellationToken ct) {
        try {
            var theme = (await _local.GetAsync<Theme>("Theme")).Value;
            if (theme is null) return;

            _theme.UpdateAll(theme);
        }
        catch(CryptographicException ex) {
            _logger.LogError(ex, "Failed to decrypt localstorage");
            await _local.DeleteAsync("Theme");
            await _local.DeleteAsync("Id");
        }
        catch (Exception e) {
            _logger.LogError(e, "Failed to read from localstorage");
        }
    }
}