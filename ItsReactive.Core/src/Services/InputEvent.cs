using ItsReactive.Core.Helpers;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using ItsReactive.Core.Helpers;
using ItsReactive.Core.Interfaces;

namespace ItsReactive.Core.Services;

public class InputEvent : IInputEvent
{
    public IInputMapper Left { get; }
    public IInputMapper Right { get; }
    public IInputMapper Up { get; }
    public IInputMapper Down { get; }
    public IInputMapper Back { get; }
    public IInputMapper PrimaryAction { get;  }
    public IInputMapper SecondaryAction { get;  }
    public IInputMapper Confirm { get;  }
    public bool FullScreenToggle { get; private set; }

    private List<KeyboardMapper> _keyboardMappers = new ();
    private List<GamePadMapper> _gamepadMappers = new ();

    public InputEvent()
    {
        Left = KeyboardGamePadMapping(new[] {Keys.Left, Keys.A}, new[] {Buttons.DPadLeft});
        Right = KeyboardGamePadMapping(new[] {Keys.Right, Keys.D}, new[] {Buttons.DPadRight});
        Up = KeyboardGamePadMapping(new[] {Keys.Up, Keys.W}, new[] {Buttons.DPadUp});
        Down = KeyboardGamePadMapping(new[] {Keys.Down, Keys.S}, new[] {Buttons.DPadDown});
        Back = KeyboardGamePadMapping(new[] {Keys.Escape}, new[] {Buttons.Y});
        PrimaryAction = KeyboardGamePadMapping(new[] {Keys.Space, Keys.L}, new[] {Buttons.A});
        SecondaryAction = KeyboardGamePadMapping(new[] {Keys.LeftControl, Keys.K}, new[] {Buttons.B});
        Confirm  = KeyboardGamePadMapping(new[] {Keys.Enter}, new[] {Buttons.A});
    }

    private CombineMapper KeyboardGamePadMapping(IEnumerable<Keys> keys, IEnumerable<Buttons> buttons)
    {
        var keyboard = new KeyboardMapper(keys.ToArray());
        var gamepad = new GamePadMapper(buttons.ToArray());
        _keyboardMappers.Add(keyboard);
        _gamepadMappers.Add(gamepad);
        return new CombineMapper(new IInputMapper[] {keyboard, gamepad});
    }
    
    public void Update()
    {
        var keyboardState = KeyboardExtended.GetState();
        var gamePadState = GamePad.GetState(0);
        _keyboardMappers.ForEach(x => x.Update(keyboardState));
        _gamepadMappers.ForEach(x => x.Update(gamePadState));

        FullScreenToggle = false;
        if (keyboardState.IsAltDown() && keyboardState.WasKeyJustDown(Keys.Enter))
        {
            FullScreenToggle = true;
        }
    }
    
}