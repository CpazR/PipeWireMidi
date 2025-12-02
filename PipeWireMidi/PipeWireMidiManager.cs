using Commons.Music.Midi;
using NLog;
using XVolume.Factory;
namespace PipeWireMidi;

class PipeWireMidiManager {

    private static readonly Logger Logger = BuildLogger();
    
    private static Logger BuildLogger() {
        // TODO: Figure out why config file isn't recognized here 
        return new LogFactory().Setup().LoadConfiguration(builder => {
                var logconsole = new NLog.Targets.ConsoleTarget("logconsole"); 
                builder.Configuration.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            })
            .GetCurrentClassLogger();
    }

    private const int MinAnalogValue = 0; // AKA boolean off
    
    private const int MaxAnalogValue = 127; // AKA boolean on

    static void Main(string[] args) {
        var volume = VolumeSubsystemFactory.Create();

        // Instantiate the MIDI input device
        var midiDeviceNumber = 16; // I think this is the client number from "aconnect -l"?

        var access = MidiAccessManager.Default;

        // TODO: Abstract out into UI dropdown list
        var device = access.Inputs.First(details => details.Id.StartsWith(midiDeviceNumber.ToString()));
        var input = access.OpenInputAsync(device.Id).Result;

        input.MessageReceived += (sender, eventArgs) => {

            if (sender is not IMidiInput senderInput) {
                Logger.Error("Received unexpected midi input");
                return;
            }
            
            // Parse midi message as byte array
            var midiData = eventArgs.Data.ToArray();
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
        };

        Logger.Info("Listening for MIDI input... Press any key to exit.");

        Console.ReadKey();
        input.CloseAsync();
    }
}