namespace Model.Entities.Theme;

public class Theme {
    public bool DarkMode { get; set; } = true;
    public string Primary { get; set; } = Colors.Green.Default;
    public string Secondary { get; set; } = Colors.Purple.Darken1;
    public ESideMenuState ESideMenuState { get; set; } = ESideMenuState.Responsive;
}