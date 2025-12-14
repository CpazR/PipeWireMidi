using DynamicData.Kernel;
using System.Diagnostics;
namespace Mixi.Audio;

/**
 * A wrapper for WirePlumber commands.
 *
 * Inspired by XVolume's many generic interfaces for linux.
 */
public class WirePlumberWrapper : IAudioWrapper {

    private static readonly string VolumeToken = "[vol:";

    private const string CommandStatus = "wpctl status";

    public static List<MediaElement> GetMediaElements() {

        // Execute the command to get the status of sinks, sources and devices
        var output = ExecuteShellCommand(CommandStatus);

        var elements = ParseAudioElements(output);
        return elements;
    }

    private static List<MediaElement> ParseAudioElements(string output) {
        var elements = new List<MediaElement>();
        var lines = output.Split([
            Environment.NewLine
        ], StringSplitOptions.RemoveEmptyEntries);

        var currentSection = MediaType.None;

        foreach (var line in lines) {
            // Set the current section based on the line content
            // TODO: Likely not scalable. Don't know how other distro's handle this.
            if (line.StartsWith(" ├─ Devices:")) {
                currentSection = MediaType.Devices;
                continue;
            }
            else if (line.StartsWith(" ├─ Sinks:")) {
                currentSection = MediaType.Sinks;
                continue;
            }
            else if (line.StartsWith(" ├─ Sources:")) {
                currentSection = MediaType.Sources;
                continue;
            }
            else if (line.StartsWith(" ├─ Filters:")) {
                currentSection = MediaType.Filters;
                continue;
            }
            else if (line.StartsWith(" └─ Streams:")) {
                currentSection = MediaType.Streams;
                continue;
            }
            else if (line.StartsWith("Video") || line.StartsWith("Settings")) {
                currentSection = MediaType.NA;
            }
            else if (line.StartsWith(" ├─") || line.StartsWith(" └─")) continue; // Skip section headers

            // Parse based on current section
            switch (currentSection) {
                case MediaType.Devices:
                case MediaType.NA:
                    // TODO: Determine if access to either of these are even useful in this context.
                    break;
                case MediaType.Sinks:
                case MediaType.Sources:
                case MediaType.Filters:
                    ParseAudio(elements, line, currentSection);
                    break;
                case MediaType.Streams:
                    ParseStreams(elements, line, currentSection);
                    break;
            }
        }

        return elements;
    }
    private static void ParseStreams(List<MediaElement> elements, string line, MediaType type) {
        var parts = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2) { // Only ID and name
            // Don't want to include the individual channels of streams.
            if (parts[1].StartsWith("output_F")) {
                return;
            }
            elements.Add(new MediaElement(parts[0].Replace(".", ""), // ID
                string.Join(" ", parts.Skip(1)), // Name
                false, // Default muted state
                1f,
                type)); // Default volume
        }
    }

    private static void ParseDevice(List<MediaElement> elements, string line, MediaType type) {
        var parts = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 3) {
            elements.Add(new MediaElement(parts[1].Replace(".", ""), // ID
                string.Join(" ", parts.Skip(2)), // Name
                false, // Default muted state
                1f,
                type)); // Default volume
        }
    }


    private static void ParseAudio(List<MediaElement> elements, string line, MediaType type) {
        var parts = line.Split([' '], StringSplitOptions.RemoveEmptyEntries).AsList();
        // Check for volume information
        if (parts.Count < 5)
            return;
        var isCurrentlyActive = parts[1] == "*";
        // Remove volume tokens. Can change between sessions.
        var volumeString = parts.Last().Replace("[vol:", "").Replace("]", "").Trim(); // Extract volume
        var volume = float.TryParse(volumeString, out var parsedVolume) ? parsedVolume : 1f; // Fallback to default
        var volumeIndex = parts.IndexOf(VolumeToken);
        // Remove twice. Should always be two whitespace separated volume tokens. Ex: [vol: 0.31]
        parts.RemoveAt(volumeIndex);
        parts.RemoveAt(volumeIndex);
        var skipId = isCurrentlyActive ? 3 : 2;
        var id = (isCurrentlyActive ? parts[2] : parts[1]).Replace(".", "");
        var name = string.Join(" ", parts.Skip(skipId)); // Name is all parts except the last two

        elements.Add(new MediaElement(id, name, false, volume, type));
    }


    public void SetVolume(string id, float volume) {
        var command = $"wpctl set-volume {id} {volume}%";
        ExecuteShellCommand(command);
        Console.WriteLine($"Set volume of {id} to {volume}%");
    }

    public void SetMute(string id, bool mute) {
        var isMuteExpression = mute ? "1" : "0";
        var command = $"wpctl set-mute {id} {isMuteExpression}";
        ExecuteShellCommand(command);
        Console.WriteLine(mute ? $"Muted {id}" : $"Unmuted {id}");
    }

    private static string ExecuteShellCommand(string command) {
        var processInfo = new ProcessStartInfo("bash", "-c \"" + command + "\"") {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = Process.Start(processInfo);
        var reader = process.StandardOutput;
        return reader.ReadToEnd();
    }

}
