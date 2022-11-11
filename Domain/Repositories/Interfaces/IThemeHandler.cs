namespace Domain.Repositories.Interfaces;

public interface IThemeHandler {
    MudTheme Theme { get; set; }
    bool ThemeMenuShown { get; set; }
    bool DarkMode { get; set; }
    bool SideOpenMini { get; set; }
    bool SideOpen { get; set; }
    ESideMenuState ESideMenuState { get; set; }

    event Action ThemeChange;
    void Rerender();
    void UpdateThemeMenu(bool shown);

    void UpdateMode(bool darkMode);
    void UpdateSideMenu(ESideMenuState state);

    void UpdateAll(Theme theme);
}