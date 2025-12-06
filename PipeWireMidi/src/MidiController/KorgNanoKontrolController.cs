using Commons.Music.Midi;
using PipeWireMidi.MidiController;
namespace PipeWireMidi;

public class KorgNanoKontrolController : AbstractMidiController {

    public KorgNanoKontrolController(IMidiPortDetails portDetails, List<MediaElement> elements) : base(portDetails) {
        // Map input to actions
        
        // TODO: Abstract all of this out to some UI system
        var mainElement = elements.First(element => element.id=="54");
        // var browserElement = elements.First(element => element.id=="90");
        
        InputConfigurations.Add(0, new InputConfiguration(0, InputType.ANALOG, InputActionType.VOLUME, mainElement));
        InputConfigurations.Add(48, new InputConfiguration(48, InputType.BUTTON, InputActionType.MUTE, mainElement));
        
        // InputConfigurations.Add(7, new InputConfiguration(7, InputType.ANALOG, InputActionType.VOLUME, browserElement));
        // InputConfigurations.Add(55, new InputConfiguration(55, InputType.BUTTON, InputActionType.MUTE, browserElement));
    }

    protected override void EventHandler(object? sender, MidiReceivedEventArgs e) {
        if (sender is not IMidiInput senderInput) {
            Logger.Error("Received unexpected midi input");
            return;
        }
            
        // Parse midi message as byte array
        var midiData = e.Data.ToArray();
        if (midiData.Length == 0) {
            Logger.Error("Received empty midi message");
            return;
        }
            
        // TODO: Open source midi device mappings to distinguish between "buttons" vs analog "sliders" or "knobs"
        int input = midiData[0];
        float value = midiData[1];
        Logger.Info($"Unkown input: Input: {input}, Value: {value}");
        if (InputConfigurations.TryGetValue(input, out var inputConfiguration)) {
            inputConfiguration.Invoke(value);
        }
    }

}
