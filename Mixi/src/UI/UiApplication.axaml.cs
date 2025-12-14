using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Input;
using Mixi.MidiController;
using Mixi.UI.MidiManager;
namespace Mixi.UI;

/**
 * The main entry class for avalonia UI
 *
 * UI will be responsible for user configuration and updating active <see cref="AbstractMidiController"/>s
 */
public partial class UiApplication : Application {

    private MidiManagerWindow? mainWindow;

    public override void Initialize() {
        AvaloniaXamlLoader.Load(this);

        // TODO: Try loading existing configuration. If not auto show midi manager window for first time setup.
    }

    public UiApplication() {
        DataContext = this;
    }

    public override void OnFrameworkInitializationCompleted() {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        }

        // Show window on application startup to establish midi controller
        BuildMidiManagerWindow();
        mainWindow?.Show();

        base.OnFrameworkInitializationCompleted();
    }

    [RelayCommand]
    public void TrayIconClicked() {
        BuildMidiManagerWindow();
        mainWindow?.Show();
    }

    [RelayCommand]
    public void ShowMidiManager() {
        BuildMidiManagerWindow();
        mainWindow?.Show();
    }

    [RelayCommand]
    public void ShowAboutWindow() {
        // TODO: Need to implement
    }

    private void BuildMidiManagerWindow() {
        mainWindow ??= new MidiManagerWindow();
        // Clear main window reference once closed
        mainWindow?.Closed += (_, _) => {
            mainWindow = null;
        };
    }

    [RelayCommand]
    public void ExitApplication() {
        switch (ApplicationLifetime) {
            case IClassicDesktopStyleApplicationLifetime desktopLifetime:
                desktopLifetime.TryShutdown();
                break;
            case IControlledApplicationLifetime controlledLifetime:
                controlledLifetime.Shutdown();
                break;
        }
    }
}
