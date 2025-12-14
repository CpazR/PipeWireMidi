using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Commons.Music.Midi;
using DynamicData.Kernel;
using Mixi.Audio;
using Mixi.MidiController;
using Mixi.UI.MidiManager.MidiControl;
using NLog;
using NLog.Targets;
using ReactiveUI.Avalonia;
namespace Mixi.UI.MidiManager;

public partial class MidiManagerWindow : ReactiveWindow<MidiManagerWindowViewModel> {

    private static readonly Logger Logger = BuildLogger();

    private static Logger BuildLogger() {
        // TODO: Figure out why config file isn't recognized here 
        return new LogFactory().Setup().LoadConfiguration(builder => {
                var logconsole = new ConsoleTarget("logconsole");
                builder.Configuration.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            })
            .GetCurrentClassLogger();
    }

    public MidiControl.MidiControl? MidiControlPanel;

    private AbstractMidiController midiController;

    public MidiManagerWindow() {
        AvaloniaXamlLoader.Load(this);
        DataContext = new MidiManagerWindowViewModel();
    }

    private void SelectedMidiDevice(object? sender, SelectionChangedEventArgs e) {
        if (e.AddedItems[0] is not IMidiPortDetails portDetail) {
            Logger.Error("Valid midi device not found: " + e.AddedItems[0]);
            return;
        }

        Logger.Info("Selected MIDI Device: " + portDetail.Name);
        midiController = MixiMidiManager.GetMidiController(portDetail);
        var mediaElements = AudioManager.GetMediaElements();

        // Load from configuration profile
        if (!string.IsNullOrEmpty(MixiMidiManager.ActiveConfiguration?.DeviceName)) {
            foreach (var activeConfigurationMidiMapping in MixiMidiManager.ActiveConfiguration.MidiMappings) {
                MixiMidiManager.ActiveConfiguration.GetMappingDetails(activeConfigurationMidiMapping.Key, out var elementName, out var elementType);
                // Get media element from active named configuration
                mediaElements
                    .FirstOrOptional(element => elementType.Equals(element.Type) &&
                                                elementName!.Equals(element.Name))
                    .IfHasValue(element => midiController.BindElement(activeConfigurationMidiMapping.Key, element));

            }
        }
        else {
            // TODO: Create configurations per-device
            if (MixiMidiManager.ActiveConfiguration != null)
                MixiMidiManager.ActiveConfiguration = MixiMidiManager.ActiveConfiguration with {
                    DeviceName = portDetail.Name
                };
            MixiMidiManager.SaveToProfile();
        }

        if (MidiControlPanel != null)
            return;

        MidiControlPanel = new KorgNanoKontrolControl((KorgNanoKontrolController)midiController, mediaElements);
        var controlPanel = this.Find<Panel>("MidiControlPanelContainer");

        if (controlPanel != null) {
            controlPanel.Children.Add(MidiControlPanel);
            Logger.Info("Midi control panel loaded");
        }
        else {
            Logger.Error("Midi control panel could not be loaded");
        }
    }

    private void CmdClose_OnClick(object? sender, RoutedEventArgs e) {
        Close();
    }
}
