using Commons.Music.Midi;
using Mixi.Audio;
namespace Mixi.MidiController;

public class KorgNanoKontrolController : AbstractMidiController {

    public KorgNanoKontrolController(IMidiPortDetails portDetails) : base(portDetails) {
        Definitions = new Dictionary<int, InputDefinition> {
            { 0, new InputDefinition(InputType.SLIDER, InputActionType.VOLUME) },
            { 1, new InputDefinition(InputType.SLIDER, InputActionType.VOLUME) },
            { 2, new InputDefinition(InputType.SLIDER, InputActionType.VOLUME) },
            { 3, new InputDefinition(InputType.SLIDER, InputActionType.VOLUME) },
            { 4, new InputDefinition(InputType.SLIDER, InputActionType.VOLUME) },
            { 5, new InputDefinition(InputType.SLIDER, InputActionType.VOLUME) },
            { 6, new InputDefinition(InputType.SLIDER, InputActionType.VOLUME) },
            { 7, new InputDefinition(InputType.SLIDER, InputActionType.VOLUME) },
            { 16, new InputDefinition(InputType.KNOB, InputActionType.VOLUME) },
            { 17, new InputDefinition(InputType.KNOB, InputActionType.VOLUME) },
            { 18, new InputDefinition(InputType.KNOB, InputActionType.VOLUME) },
            { 19, new InputDefinition(InputType.KNOB, InputActionType.VOLUME) },
            { 20, new InputDefinition(InputType.KNOB, InputActionType.VOLUME) },
            { 21, new InputDefinition(InputType.KNOB, InputActionType.VOLUME) },
            { 22, new InputDefinition(InputType.KNOB, InputActionType.VOLUME) },
            { 23, new InputDefinition(InputType.KNOB, InputActionType.VOLUME) },
            { 48, new InputDefinition(InputType.BUTTON, InputActionType.MUTE) },
            { 49, new InputDefinition(InputType.BUTTON, InputActionType.MUTE) },
            { 50, new InputDefinition(InputType.BUTTON, InputActionType.MUTE) },
            { 51, new InputDefinition(InputType.BUTTON, InputActionType.MUTE) },
            { 52, new InputDefinition(InputType.BUTTON, InputActionType.MUTE) },
            { 53, new InputDefinition(InputType.BUTTON, InputActionType.MUTE) },
            { 54, new InputDefinition(InputType.BUTTON, InputActionType.MUTE) },
            { 55, new InputDefinition(InputType.BUTTON, InputActionType.MUTE) },
        };
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

    public override void BindElement(int input, MediaElement element) {
        // If ID is blank, remove any configurations
        if (string.IsNullOrEmpty(element.Id)) {
            InputConfigurations.Remove(input);
            return;
        }
        var configuration = new InputConfiguration(input, Definitions[input], element);
        InputConfigurations[input] = configuration;
        // TODO: Handle saving to external file here.
    }

}
