## Mixi Audio Manager

A cross-platform midi volume manager. Heavily inspired by [midi mixer](https://www.midi-mixer.com/) by Jack
Williams.

Originally practice for learning C#, this project was open sourced to allow for future expansion for better cross
platform support as well as better support for more midi devices.

This application was explicitly designed to be scalable for new devices and audio interfaces for different platforms.

### Screenshots

![Screenshot_20251207_170903.png](Media/Screenshot_20251207_170903.png)

![Screenshot_20251207_170932.png](Media/Screenshot_20251207_170932.png)

![Screenshot_20251207_171017.png](Media/Screenshot_20251207_171017.png)

### Planned features

- [ ] Working UI using Avalonia for cross platform support
    - [x] Taskbar "service"
    - [x] Midi control window
    - [x] Save/load for UI
        - Needs to be cross platform tested. _Should work_. But needs verification.
    - [ ] Device Profiles
        - Partially funcitoning. Need to be able to create/load profiles based on device name.
    - [ ] Polish
- [x] Audio abstraction layer
- [ ] OS Audio interface layers
    - [ ] Linux (Needs to include a few different implementations such as pulse audio, pipewire, etc)
        - [x] WirePlumber
        - [ ] PipeWire (May be covered by wire plumber)
        - [ ] Pulse Audio
        - [ ] ALSA
    - [ ] Windows
    - [ ] MacOs
- [ ] Midi device support
    - [x] Korg Nano Kontrol 2
    - Need to expand list or figure out a way to generically fetch infromation from midi devices