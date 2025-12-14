using NLog;
using NLog.Targets;
using ReactiveUI.Avalonia;
namespace Mixi.UI.MidiManager.MidiControl;

public abstract class MidiControl : ReactiveUserControl<MidiControlViewModel> {
    protected static readonly Logger Logger = BuildLogger();
    private static Logger BuildLogger() {

        // TODO: Figure out why config file isn't recognized here 
        return new LogFactory().Setup().LoadConfiguration(builder => {
                var logconsole = new ConsoleTarget("logconsole");
                builder.Configuration.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            })
            .GetCurrentClassLogger();
    }
}
