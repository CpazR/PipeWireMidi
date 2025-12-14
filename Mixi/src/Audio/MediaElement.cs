namespace Mixi.Audio;

public record MediaElement(string Id, string Name, bool IsMuted, float Volume, MediaType Type) {

    public bool IsMuted { get; set; } = IsMuted;

    public string DisplayName { get => $"{Type.ToString()} : {Name}"; }
}

/**
 * TODO: Not applicable to all platforms.
 */
public enum MediaType {
    None,
    Devices,
    Sinks,
    Sources,
    Filters,
    Streams,
    NA
}
