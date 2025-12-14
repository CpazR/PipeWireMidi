using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Mixi.Audio;
using Mixi.MidiController;
namespace Mixi.UI.MidiManager.MidiControl;

/**
 * The UI interface for a KorgNanoKontrol2
 */
public partial class KorgNanoKontrolControl : MidiControl {

    private readonly KorgNanoKontrolController _midiController;

    private readonly List<MediaElement> _elements;

    public KorgNanoKontrolControl(KorgNanoKontrolController midiController, List<MediaElement> elements) {
        _midiController = midiController;
        _elements = elements;
        AvaloniaXamlLoader.Load(this);

        DataContext = new MidiControlViewModel(midiController.Definitions, elements);
    }

    private void SelectedMedia(object? sender, SelectionChangedEventArgs e) {
        if (e.AddedItems.Count == 0 || e.AddedItems[0] is not MediaElement selectedElement) {
            Logger.Error("Valid midi device not found: " + e.AddedItems);
            return;
        }

        if (sender is not ComboBox inputDropdown) {
            Logger.Error("Invalid sender: " + sender);
            return;
        }
        var dropdownInputId = inputDropdown.GetValue(MidiInputComboBoxHelper.MidiInputIdProperty);

        if (string.IsNullOrEmpty(selectedElement.Id)) {
            inputDropdown.SelectedItem = null;
        }

        _midiController?.BindElement(dropdownInputId, selectedElement);
        MixiMidiManager.ActiveConfiguration?.SetMapping(dropdownInputId, selectedElement);
        MixiMidiManager.SaveToProfile();
        Logger.Info("Selected media " + selectedElement.Name);
    }
}
