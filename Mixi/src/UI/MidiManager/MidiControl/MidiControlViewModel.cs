using DynamicData.Kernel;
using Mixi.Audio;
using Mixi.Configuration;
using Mixi.MidiController;
using ReactiveUI;
using System.Reactive.Disposables;
namespace Mixi.UI.MidiManager.MidiControl;

/**
 * A generic view model for <see cref="MidiControl"/>
 */
public class MidiControlViewModel : ReactiveObject, IActivatableViewModel {

    private IList<ComboBoxInput> _comboBoxInputs = new List<ComboBoxInput>();

    public IList<ComboBoxInput> ComboBoxInputs
    {
        get => _comboBoxInputs;
        set => this.RaiseAndSetIfChanged(ref _comboBoxInputs, value);
    }

    public ViewModelActivator Activator { get; } = new();

    public MidiControlViewModel(Dictionary<int, InputDefinition> definitions, List<MediaElement> mediaElements) {

        this.WhenActivated(disposables => {
            // Initial empty element
            mediaElements.Insert(0, new MediaElement("", "", false, 0, MediaType.None));
            var inputs = definitions.Select(keyValuePair => keyValuePair.Key)
                .Select(inputNumber => new ComboBoxInput(inputNumber, definitions[inputNumber].Type, mediaElements))
                .ToList();

            ComboBoxInputs = inputs;

            disposables.Add(Disposable.Empty);

            foreach (var comboBoxInput in ComboBoxInputs) {
                comboBoxInput.Activator.Activate();
            }
        });
    }
}

public class ComboBoxInput : ReactiveObject, IActivatableViewModel {
    private int _inputNumber;

    public int InputNumber
    {
        get => _inputNumber;
        set => this.RaiseAndSetIfChanged(ref _inputNumber, value);
    }

    private InputType _inputType;

    public InputType InputType
    {
        get => _inputType;
        set => this.RaiseAndSetIfChanged(ref _inputType, value);
    }

    private MediaElement? _selectedMediaElement;

    public MediaElement? SelectedMediaElement
    {
        get => _selectedMediaElement;
        set => this.RaiseAndSetIfChanged(ref _selectedMediaElement, value);
    }

    private IList<MediaElement> _mediaElements;

    public IList<MediaElement> MediaElements
    {
        get => _mediaElements;
        set => this.RaiseAndSetIfChanged(ref _mediaElements, value);
    }

    public ViewModelActivator Activator { get; } = new();

    public ComboBoxInput(int inputNumber, InputType type, List<MediaElement> mediaElements) {
        _inputNumber = inputNumber;
        _inputType = type;
        _mediaElements = mediaElements;

        this.WhenActivated(disposables => {

            if (MixiMidiManager.ActiveConfiguration != null) {
                DefaultSelection(MixiMidiManager.ActiveConfiguration, MediaElements);
            }
            disposables.Add(Disposable.Empty);
        });
    }

    public void DefaultSelection(ActiveConfiguration configuration, IList<MediaElement> mediaElements) {
        configuration.GetMappingDetails(InputNumber, out var elementName, out var elementType);
        mediaElements
            .FirstOrOptional(element => element.Type.Equals(elementType) &&
                                        element.Name.Equals(elementName))
            .IfHasValue(element => SelectedMediaElement = element);
    }
}
