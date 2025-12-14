using Avalonia;
using Commons.Music.Midi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mixi.Configuration;
using Mixi.MidiController;
using Mixi.UI;
using NLog;
using NLog.Targets;
using ReactiveUI.Avalonia;
using System.Text.Json;
namespace Mixi;

class MixiMidiManager {

    private static readonly Logger Logger = BuildLogger();

    private static Logger BuildLogger() {
        // TODO: Figure out why config file isn't recognized here 
        return new LogFactory().Setup().LoadConfiguration(builder => {
                var logconsole = new ConsoleTarget("logconsole");
                builder.Configuration.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            })
            .GetCurrentClassLogger();
    }

    public const string ProfileFileName = "Profile.json";

    public static readonly string Path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Mixi";

    public static readonly string FilePath = $"{Path}/{ProfileFileName}";


    private static AbstractMidiController? activeMidiController;

    public static ActiveConfiguration? ActiveConfiguration;

    [STAThread]
    static void Main(string[] args) {
        Logger.Info("Checking for existing profile");

        if (!LoadFromProfile()) {
            Logger.Info($"No profile found. Creating empty configuration at - {Path}");
            Directory.CreateDirectory(Path);
            File.Create(FilePath).Dispose();

            ActiveConfiguration = new ActiveConfiguration("", new Dictionary<int, string>());
            File.WriteAllText(FilePath, JsonSerializer.Serialize(ActiveConfiguration));
        }

        // Logger.Info("Setting up midi service daemon:");
        // var daemonHost = BuildMidiDaemonWorker();
        // daemonHost.RunAsync();


        Logger.Info("Standing up Ui");
        var uiApp = BuildAvaloniaApp();

        // TODO: This should block the main thread. But shutdown daemon host on UI shutdown. 
        uiApp.StartWithClassicDesktopLifetime(args);
        activeMidiController?.Close();
        // daemonHost.StopAsync();
        // daemonHost.WaitForShutdown();
    }

    private static IHost BuildMidiDaemonWorker() {
        var applicationBuilder = Host.CreateApplicationBuilder();
        applicationBuilder.Services.AddHostedService<MidiWorker>();

        var host = applicationBuilder.Build();
        host.RunAsync();
        return host;
    }

    public static bool LoadFromProfile() {
        if (!File.Exists(FilePath)) {
            return false;
        }
        Logger.Info($"Loading profile from - {FilePath}");
        var json = File.ReadAllText(FilePath);
        ActiveConfiguration = JsonSerializer.Deserialize<ActiveConfiguration>(json);
        return true;
    }

    public static bool SaveToProfile() {
        Logger.Info($"Saving profile to - {FilePath}");
        var json = JsonSerializer.Serialize(ActiveConfiguration);
        File.WriteAllText(FilePath, json);
        return true;
    }

    public static AppBuilder BuildAvaloniaApp() {
        return AppBuilder.Configure<UiApplication>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();
    }
    public static AbstractMidiController GetMidiController(IMidiPortDetails portDetails) {
        // TODO: May need to use the worker appraoch here. Need to do testing on this this would be managed with the UI piece and managed midi.
        activeMidiController ??= new KorgNanoKontrolController(portDetails);
        // TODO: Need to standardize a list of midi device names and confirm consistency across platforms.
        // This list could be used to map names to controller implementations
        return activeMidiController;
    }

}
