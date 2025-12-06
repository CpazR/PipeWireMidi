using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Targets;
using PipeWireMidi.UI;
namespace PipeWireMidi;

class PipeWireMidiManager {

    private static readonly Logger Logger = BuildLogger();

    private static Logger BuildLogger() {
        // TODO: Figure out why config file isn't recognized here 
        return new LogFactory().Setup().LoadConfiguration(builder => {
                var logconsole = new ConsoleTarget("logconsole");
                builder.Configuration.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            })
            .GetCurrentClassLogger();
    }


    [STAThread]
    static void Main(string[] args) {
        Logger.Info("Setting up midi service daemon:");
        var daemonHost = BuildMidiDaemonWorker();
        daemonHost.RunAsync();

        Logger.Info("Standing up Ui");
        var uiApp = BuildAvaloniaUi();

        // TODO: This should block the main thread. But shutdown daemon host on UI shutdown. 
        uiApp.StartWithClassicDesktopLifetime(args);
        daemonHost.StopAsync();
        daemonHost.WaitForShutdown();
    }

    private static IHost BuildMidiDaemonWorker() {
        var applicationBuilder = Host.CreateApplicationBuilder();
        applicationBuilder.Services.AddHostedService<MidiWorker>();

        var host = applicationBuilder.Build();
        host.RunAsync();
        return host;
    }

    private static AppBuilder BuildAvaloniaUi() {
        return AppBuilder.Configure<UiApplication>()
            .UsePlatformDetect()
            .LogToTrace();
    }
}
