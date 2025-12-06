using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Input;
using PipeWireMidi.MidiController;
using PipeWireMidi.UI.MidiManager;
namespace PipeWireMidi.UI;

/**
 * The main entry class for avalonia UI
 *
 * UI will be responsible for user configuration and updating active <see cref="AbstractMidiController"/>s
 */
public partial class UiApplication : Application {

    private MidiManagerWindow? mainWindow;

    public override void Initialize() {
        AvaloniaXamlLoader.Load(this);
    }

    public UiApplication() {
        DataContext = this;
    }

    public override void OnFrameworkInitializationCompleted() {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        }

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
        mainWindow?.Closed += (_, _) => mainWindow = null;
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
