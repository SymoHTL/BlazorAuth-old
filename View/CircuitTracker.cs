using Model.Entities.Theme;

namespace View;

public class CircuitTracker : CircuitHandler {
    private readonly ProtectedLocalStorage _local;

    private readonly IThemeHandler _theme;

    public CircuitTracker(IThemeHandler theme, ProtectedLocalStorage local) {
        _theme = theme;
        _local = local;
    }

    public override async Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken) {
        await GetStuff();
    }

    private async Task GetStuff() {
        try {
            var theme = (await _local.GetAsync<Theme>("Theme")).Value;
            if (theme is null) return;

            _theme.UpdateAll(theme);
        }
        catch (Exception e) {
            if (e is CryptographicException) {
                Console.WriteLine(e);
                Console.WriteLine(
                    "####################################################################################");
                Console.WriteLine("                         LocalStorage is now getting deleted!");
                Console.WriteLine(
                    "####################################################################################");
                await _local.DeleteAsync("Theme");
            }
            else {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}