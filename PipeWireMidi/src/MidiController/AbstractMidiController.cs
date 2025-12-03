using Commons.Music.Midi;
using NLog;
using NLog.Targets;
using XVolume.Abstractions;
using XVolume.Factory;
namespace PipeWireMidi.MidiController;

public abstract class AbstractMidiController {
    
    protected static readonly Logger Logger = BuildLogger();
    
    private static Logger BuildLogger() {
        // TODO: Figure out why config file isn't recognized here 
        return new LogFactory().Setup().LoadConfiguration(builder => {
                var logconsole = new ConsoleTarget("logconsole"); 
                builder.Configuration.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            })
            .GetCurrentClassLogger();
    }
    
    protected static IVolumeSubsystem volume = VolumeSubsystemFactory.Create();

    protected const int MinAnalogValue = 0; // AKA boolean off

    protected const int MaxAnalogValue = 127; // AKA boolean on

    private IMidiInput input;
    
    protected AbstractMidiController(IMidiPortDetails portDetails) {
        var access = MidiAccessManager.Default;
        input = access.OpenInputAsync(portDetails.Id).Result;
        
        input.MessageReceived += EventHandler;
    }

    protected abstract void EventHandler(object? sender, MidiReceivedEventArgs e);

    public void Close() {
        input.CloseAsync();
    }
}