using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Targets;
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

   static void Main(string[] args) {

        Logger.Info("Setting up midi service daemon:");

        var applicationBuilder = Host.CreateApplicationBuilder();
        applicationBuilder.Services.AddHostedService<MidiWorker>();

        var host = applicationBuilder.Build();
        host.Run();
   }
}