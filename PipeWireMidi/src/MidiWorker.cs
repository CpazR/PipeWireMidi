using Commons.Music.Midi;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Targets;
namespace PipeWireMidi;

public class MidiWorker : IHostedService {

    private static readonly Logger Logger = BuildLogger();
    
    private static Logger BuildLogger() {
        // TODO: Figure out why config file isn't recognized here 
        return new LogFactory().Setup().LoadConfiguration(builder => {
                var logconsole = new ConsoleTarget("logconsole"); 
                builder.Configuration.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            })
            .GetCurrentClassLogger();
    }
    
    private KorgNanoKontrolController controller;

    public Task StartAsync(CancellationToken cancellationToken) {
        // Instantiate the MIDI input device
        // TODO: Pull this out into a UI

        var midiDeviceNumber = "28_0"; // I think this is the client number from "aconnect -l"?

        var access = MidiAccessManager.Default;
       
        Logger.Debug(access.Inputs);
        Logger.Debug(access.Outputs);
        
        var elements = WirePlumberWrapper.GetMediaElements();
        var midiPortDetail = access.Inputs.First(details => details.Id == midiDeviceNumber);
        controller = new KorgNanoKontrolController(midiPortDetail, elements);
        
        return Task.CompletedTask;
    }
    
    public Task StopAsync(CancellationToken cancellationToken) {
        controller.Close();
        return Task.CompletedTask;
    }
}
