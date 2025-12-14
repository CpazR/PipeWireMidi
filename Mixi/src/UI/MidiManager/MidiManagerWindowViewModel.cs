using Commons.Music.Midi;
using ReactiveUI;
using System.Reactive.Disposables;
namespace Mixi.UI.MidiManager;

public class MidiManagerWindowViewModel : ReactiveObject, IActivatableViewModel {

    private IMidiPortDetails? _selectedPortDetails;

    public IMidiPortDetails? SelectedPortDetails
    {
        get => _selectedPortDetails;
        set => this.RaiseAndSetIfChanged(ref _selectedPortDetails, value);
    }

    private IList<IMidiPortDetails>? _portDetails = new List<IMidiPortDetails>();

    public IList<IMidiPortDetails> PortDetails
    {
        get => _portDetails;
        set => this.RaiseAndSetIfChanged(ref _portDetails, value);
    }

    public ViewModelActivator Activator { get; } = new();

    public MidiManagerWindowViewModel() {
        this.WhenActivated(disposables => {
            var midiAccess = MidiAccessManager.Default;
            PortDetails = midiAccess.Inputs.ToList();
            DefaultSelection();

            disposables.Add(Disposable.Empty);
        });
    }

    private void DefaultSelection() {
        // Attempt to load configuration from profile
        var savedDeviceName = MixiMidiManager.ActiveConfiguration?.DeviceName;
        if (string.IsNullOrEmpty(savedDeviceName))
            return;
        var deviceDetails = PortDetails.First(element => element.Name.Equals(savedDeviceName));
        SelectedPortDetails = deviceDetails;
    }
}
