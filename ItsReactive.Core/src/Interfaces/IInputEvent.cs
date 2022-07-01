using ItsReactive.Core.Helpers;

namespace ItsReactive.Core.Interfaces;

public interface IInputEvent
{
     IInputMapper Left { get; }
     IInputMapper Right { get; }
     IInputMapper Up { get; }
     IInputMapper Down { get; }
     IInputMapper Back { get; }
     IInputMapper PrimaryAction { get; }
     IInputMapper SecondaryAction { get; }
     IInputMapper Confirm { get; }
     bool FullScreenToggle { get; }

     void Update();
}