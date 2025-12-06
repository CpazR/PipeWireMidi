using Avalonia.Controls;
using Avalonia.Interactivity;
namespace PipeWireMidi.UI.MidiManager;

public partial class MidiManagerWindow : Window {

    private void CmdClose_OnClick(object? sender, RoutedEventArgs e) {
        Close();
    }
}
