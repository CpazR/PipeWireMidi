using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Input;
using PipeWireMidi.MidiController;
namespace PipeWireMidi.UI;

/**
 * The main entry class for avalonia UI
 *
 * UI will be responsible for user configuration and updating active <see cref="AbstractMidiController"/>s
 */
public partial class UiApplication : Application {

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
        // TODO: Need to implement
    }

    [RelayCommand]
    public void ShowMidiManager() {
        // TODO: Need to implement
    }

    [RelayCommand]
    public void ShowAboutWindow() {
        // TODO: Need to implement
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
