using Microsoft.Xna.Framework.Input;
using ItsReactive.Core.Helpers;

namespace ItsReactive.Core.Helpers;

public class GamePadMapper : IInputMapper
{
    public bool IsDown { get; private set; }
    public bool IsUp { get; private set; }
    public bool IsPressed { get; private set; }
    
    private Buttons[] _target;
    private Dictionary<Buttons, bool> _oldState = new ();

    public GamePadMapper(Buttons target)
    {
        _target = new []{target};
    }
    
    public GamePadMapper(Buttons[] target)
    {
        _target = target;
        foreach (var button in target)
        {
            _oldState[button] = false;
        }
    }

    public void Update(GamePadState state)
    {
        Reset();

        foreach (var button in _target)
        {
            if (state.IsButtonDown(button))
                IsDown = true;
            if (state.IsButtonUp(button))
                IsUp = true;
            if (_oldState[button] && state.IsButtonUp(button))
                IsPressed = true;
            
            _oldState[button] = state.IsButtonDown(button);
        }
        
    }

    public void Reset()
    {
        IsDown = false;
        IsUp = false;
        IsPressed = false;
    }
}