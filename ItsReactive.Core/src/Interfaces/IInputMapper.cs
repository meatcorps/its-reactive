using MonoGame.Extended.Input;

namespace ItsReactive.Core.Helpers;

public interface IInputMapper
{
    bool IsDown { get; }
    bool IsUp { get; }
    bool IsPressed { get; }
}