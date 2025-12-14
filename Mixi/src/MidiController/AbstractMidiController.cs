using Commons.Music.Midi;
using Mixi.Audio;
using NLog;
using NLog.Targets;
using XVolume.Abstractions;
using XVolume.Factory;
namespace Mixi.MidiController;

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

    /**
     * Generically manages each input's action.
     * For example, some slider is input "12", which performs the action of assigning the volume the value of that input.
     */
    protected readonly Dictionary<int, InputConfiguration> InputConfigurations = new();

    /**
     * Defines a mapping of each input index to a definition of that inputs makeup for a given device.
     * For example, defines if the input from a given index is analog or digital.
     *
     * Needs to be defined per-device
     */
    public Dictionary<int, InputDefinition> Definitions { get; protected init; }

    private static readonly IVolumeSubsystem volume = VolumeSubsystemFactory.Create();

    public const int MinAnalogValue = 0; // AKA boolean off

    public const int MaxAnalogValue = 127; // AKA boolean on

    private IMidiInput input;

    protected AbstractMidiController(IMidiPortDetails portDetails) {
        var access = MidiAccessManager.Default;
        input = access.OpenInputAsync(portDetails.Id).Result;

        input.MessageReceived += EventHandler;
    }

    protected abstract void EventHandler(object? sender, MidiReceivedEventArgs e);

    public abstract void BindElement(int input, MediaElement element);

    protected void SetVolume(float value) {
        float scaledValue = (value / MaxAnalogValue) * 100;
        volume.SetVolumeSmooth((int)scaledValue, 100);
    }

    protected void ToggleMute(int value) {
        if (value == MaxAnalogValue) {
            volume.Mute();
        }
        else {
            volume.Unmute();
        }
    }

    public void Close() {
        input.CloseAsync();
    }
}
