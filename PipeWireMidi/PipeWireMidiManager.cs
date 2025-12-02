using Commons.Music.Midi;
using NLog;
using System.Net.Sockets;
using System.Text;
using XVolume.Factory;

namespace PipeWireMidi;

class PipeWireMidiManager {
    
    private static readonly Logger Logger = new LogFactory().GetCurrentClassLogger();
    
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
            // TODO: Somehow parse data being received here...
            Logger.Debug(eventArgs);
            
        };

        Logger.Info("Listening for MIDI input... Press any key to exit.");

        Console.ReadKey();
        input.CloseAsync();
    }
}