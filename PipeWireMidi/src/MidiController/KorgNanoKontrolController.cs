using Commons.Music.Midi;
using PipeWireMidi.MidiController;
namespace PipeWireMidi;

public class KorgNanoKontrolController : AbstractMidiController {

    public KorgNanoKontrolController(IMidiPortDetails portDetails) : base(portDetails){}

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
        byte input = midiData[0];
        float value = midiData[1];
        Logger.Debug($"Unkown input: Input: {input}, Value: {value}");
        float scaledValue = (value / MaxAnalogValue) * 100;
        volume.SetVolumeSmooth((int) scaledValue, 100);
    }

}
