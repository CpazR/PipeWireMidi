using Mixi.Audio;
namespace Mixi.Configuration;

public record ActiveConfiguration(string DeviceName, Dictionary<int, string> MidiMappings) {

    /**
     * Add a key/value pair of the input ID and the media element name
     */
    public void SetMapping(int key, MediaElement value) {
        MidiMappings[key] = $"{value.Type} : {value.Name}";
    }

    /**
     * Get name and media type from input ID.
     * Name should never be null. But want to avoid hard crashing.
     */
    public void GetMappingDetails(int key, out string? name, out MediaType type) {
        if (!MidiMappings.TryGetValue(key, out var fullName)) {
            name = null;
            type = MediaType.None;
            return;
        }
        var parts = fullName.Split([" : "], StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2) {
            // Need to log error here.
            name = null;
            type = MediaType.None;
            return;
        }
        Enum.TryParse(parts[0], out type);
        name = parts[1];
    }
}
