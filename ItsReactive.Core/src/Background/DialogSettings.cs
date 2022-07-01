using System.Reactive.Subjects;
using ItsReactive.Core.Helpers;
using ItsReactive.Core.Helpers;

namespace ItsReactive.Core.Background;

public struct DialogSettings
{
    public string Message;
    public double Duration;
    public DialogType Type;
    public TileSetList Icon;
    public Subject<string> OnClosing = new ();

    public DialogSettings(string message, double duration, DialogType type, TileSetList icon) : this()
    {
        Message = message;
        Duration = duration;
        Type = type;
        Icon = icon;
    }
}

public enum DialogType
{
    Message,
    Popup
}