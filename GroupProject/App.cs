using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;

namespace Chess;

public class App : Application
{
    public override void Initialize()
    {
        var fluent = new Avalonia.Themes.Fluent.FluentTheme();

        Styles styles = new Styles();
        styles.Add(fluent);
        Application.Current?.Styles.Add(styles);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
