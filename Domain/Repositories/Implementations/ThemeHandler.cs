namespace Domain.Repositories.Implementations;

public class ThemeHandler : IThemeHandler {
    private readonly Palette _darkPalette = new() {
        Black = "#27272f",
        Background = "rgb(21,27,34)",
        BackgroundGrey = "#27272f",
        Surface = "#212B36",
        DrawerBackground = "#13171c",
        DrawerText = "rgba(255,255,255, 0.50)",
        DrawerIcon = "rgba(255,255,255, 0.50)",
        AppbarBackground = "#202024",
        AppbarText = "rgba(255,255,255, 0.70)",
        TextPrimary = "rgba(255,255,255, 0.70)",
        TextSecondary = "rgba(255,255,255, 0.50)",
        ActionDefault = "#adadb1",
        ActionDisabled = "rgba(255,255,255, 0.26)",
        ActionDisabledBackground = "rgba(255,255,255, 0.12)",
        Divider = "rgba(255,255,255, 0.12)",
        DividerLight = "rgba(255,255,255, 0.06)",
        TableLines = "rgba(255,255,255, 0.12)",
        LinesDefault = "rgba(255,255,255, 0.12)",
        LinesInputs = "rgba(255,255,255, 0.3)",
        TextDisabled = "rgba(255,255,255, 0.2)"
    };

    private readonly Palette _lightPalette = new();

    public MudTheme Theme { get; set; } = new() {
        Palette = new Palette {
            Black = "#27272f",
            Background = "rgb(21,27,34)",
            BackgroundGrey = "#27272f",
            Surface = "#212B36",
            DrawerBackground = "#13171c",
            DrawerText = "rgba(255,255,255, 0.50)",
            DrawerIcon = "rgba(255,255,255, 0.50)",
            AppbarBackground = "#202024",
            AppbarText = "rgba(255,255,255, 0.70)",
            TextPrimary = "rgba(255,255,255, 0.70)",
            TextSecondary = "rgba(255,255,255, 0.50)",
            ActionDefault = "#adadb1",
            ActionDisabled = "rgba(255,255,255, 0.26)",
            ActionDisabledBackground = "rgba(255,255,255, 0.12)",
            Divider = "rgba(255,255,255, 0.12)",
            DividerLight = "rgba(255,255,255, 0.06)",
            TableLines = "rgba(255,255,255, 0.12)",
            LinesDefault = "rgba(255,255,255, 0.12)",
            LinesInputs = "rgba(255,255,255, 0.3)",
            TextDisabled = "rgba(255,255,255, 0.2)"
        },
        LayoutProperties = new LayoutProperties {
            AppbarHeight = "80px",
            DefaultBorderRadius = "12px"
        },
        Typography = new Typography {
            Default = new Default {
                FontSize = "0.9rem"
            }
        },
        ZIndex = new ZIndex {
            Snackbar = 20000
        }
    };

    public event Action? ThemeChange;
    public bool ThemeMenuShown { get; set; }
    public bool DarkMode { get; set; } = true;

    public bool SideOpenMini { get; set; } = true;
    public bool SideOpen { get; set; }
    public ESideMenuState ESideMenuState { get; set; } = ESideMenuState.Responsive;

    public void Rerender() {
        ThemeChange?.Invoke();
    }


    public void UpdateThemeMenu(bool shown) {
        ThemeMenuShown = shown;
        ThemeChange?.Invoke();
    }

    public void UpdateMode(bool darkMode) {
        DarkMode = darkMode;
        Theme.Palette = darkMode ? _darkPalette : _lightPalette;
        ThemeChange?.Invoke();
    }

    public void UpdateSideMenu(ESideMenuState state) {
        switch (state) {
            case ESideMenuState.Minimized:
                SideOpenMini = false;
                SideOpen = false;
                break;
            case ESideMenuState.Responsive:
                SideOpenMini = true;
                SideOpen = false;
                break;
            case ESideMenuState.Maximized:
                SideOpenMini = false;
                SideOpen = true;
                break;
            default:
                throw new NotSupportedException();
        }

        ESideMenuState = state;
        ThemeChange?.Invoke();
    }

    public void UpdateAll(Theme theme) {
        DarkMode = theme.DarkMode;
        Theme.Palette = theme.DarkMode ? _darkPalette : _lightPalette;
        Theme.Palette.Primary = theme.Primary;
        Theme.Palette.Secondary = theme.Secondary;
        UpdateSideMenu(theme.ESideMenuState);
    }
}