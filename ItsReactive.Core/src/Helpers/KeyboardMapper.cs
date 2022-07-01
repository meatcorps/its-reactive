using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using ItsReactive.Core.Helpers;

namespace ItsReactive.Core.Helpers;

public class KeyboardMapper : IInputMapper
{
    public bool IsDown { get; private set; }
    public bool IsUp { get; private set; }
    public bool IsPressed { get; private set; }
    private Keys[] _target;

    public KeyboardMapper(Keys target)
    {
        _target = new []{target};
    }
    
    public KeyboardMapper(Keys[] target)
    {
        _target = target;
    }

    public void Update(KeyboardStateExtended state)
    {
        Reset();

        foreach (var key in _target)
        {
            if (state.IsKeyDown(key))
                IsDown = true;
            if (state.IsKeyUp(key))
                IsUp = true;
            if (state.WasKeyJustDown(key))
                IsPressed = true;
        }
    }

    public void Reset()
    {
        IsDown = false;
        IsUp = false;
        IsPressed = false;
    }
}