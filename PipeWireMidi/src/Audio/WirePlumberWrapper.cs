using System.Diagnostics;
namespace PipeWireMidi;

// Enums to track current sections
enum Section {
    None,
    Devices,
    Sinks,
    Sources,
    Filters,
    Streams
}

/**
 * A wrapper for WirePlumber commands.
 *
 * Inspired by XVolume's
 */
public class WirePlumberWrapper : IAudioWrapper {

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

        var currentSection = Section.None;
        var readingStreams = false;

        foreach (var line in lines) {
            // Set the current section based on the line content
            // TODO: Likely not scalable. Don't know how other distro's handle this.
            if (line.StartsWith(" ├─ Devices:")) {
                currentSection = Section.Devices;
                continue;
            }
            else if (line.StartsWith(" ├─ Sinks:")) {
                currentSection = Section.Sinks;
                continue;
            }
            else if (line.StartsWith(" ├─ Sources:")) {
                currentSection = Section.Sources;
                continue;
            }
            else if (line.StartsWith(" ├─ Filters:")) {
                currentSection = Section.Filters;
                continue;
            }
            else if (line.StartsWith(" └─ Streams:")) {
                currentSection = Section.Streams;
                continue;
            }
            else if (line.StartsWith(" ├─") || line.StartsWith(" └─")) continue; // Skip section headers

            // Parse based on current section
            switch (currentSection) {
                case Section.Devices:
                    ParseDevice(elements, line);
                    break;
                case Section.Sinks:
                case Section.Sources:
                case Section.Filters:
                    ParseAudio(elements, line);
                    break;
                case Section.Streams:
                    ParseStreams(elements, line);
                    break;
            }
        }

        return elements;
    }
    private static void ParseStreams(List<MediaElement> elements, string line) {
        var parts = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2) { // Only ID and name
            elements.Add(new MediaElement(parts[0].Replace(".", ""), // ID
                string.Join(" ", parts.Skip(1)), // Name
                false, // Default muted state
                1f)); // Default volume
        }
    }

    private static void ParseDevice(List<MediaElement> elements, string line) {
        var parts = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 3) {
            elements.Add(new MediaElement(parts[1].Replace(".", ""), // ID
                string.Join(" ", parts.Skip(2)), // Name
                false, // Default muted state
                1f)); // Default volume
        }
    }
    

    private static void ParseAudio(List<MediaElement> elements, string line) {
        var parts = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);
        // Check for volume information
        if (parts.Length < 5)
            return;
        // TODO: Is noting active devices useful? Don't think so.
        var isCurrentlyActive = parts[1] == "*";
        var id = (isCurrentlyActive ? parts[2] : parts[1]).Replace(".", "");
        var name = string.Join(" ", parts.Skip(2).Take(parts.Length - 3)); // Name is all parts except the last two
        var volumeString = parts.Last().Replace("[vol:", "").Replace("]", "").Trim(); // Extract volume
        var volume = float.TryParse(volumeString, out var parsedVolume) ? parsedVolume : 1f; // Fallback to default

        elements.Add(new MediaElement(id, name, false, volume));
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
