using Commons.Music.Midi;
using Microsoft.Extensions.Hosting;
using Mixi.Audio;
using Mixi.MidiController;
using NLog;
using NLog.Targets;
namespace Mixi;

/**
 * TODO: May be a deprecated approach. UI is more flexible than expected.

 */
public class MidiWorker(AbstractMidiController controller) : IHostedService {

    private static readonly Logger Logger = BuildLogger();

    private static Logger BuildLogger() {
        // TODO: Figure out why config file isn't recognized here 
        return new LogFactory().Setup().LoadConfiguration(builder => {
                var logconsole = new ConsoleTarget("logconsole");
                builder.Configuration.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            })
            .GetCurrentClassLogger();
    }

    public Task StartAsync(CancellationToken cancellationToken) {
        // Instantiate the MIDI input device
        // TODO: Pull this out into a UI
        // NOTE: These IDs are volatile and will change on device reboot. Closest thing to an ID would probably be the device name.
        var midiDeviceNumber = "28_0"; // I think this is the client number from "aconnect -l"?

        var access = MidiAccessManager.Default;

        Logger.Debug(access.Inputs);
        Logger.Debug(access.Outputs);

        var elements = WirePlumberWrapper.GetMediaElements();
        // var midiPortDetail = access.Inputs.First(details => details.Id == midiDeviceNumber);
        // controller = new KorgNanoKontrolController(midiPortDetail, elements);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        if (controller != null) {
            controller.Close();
        }
        return Task.CompletedTask;
    }
}
