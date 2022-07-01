using ItsReactive.Core.Helpers;

namespace ItsReactive.Core.Helpers;

public class CombineMapper : IInputMapper
{
    public bool IsDown => _targets.FindIndex(x => x.IsDown) > -1;

    public bool IsUp => _targets.FindIndex(x => x.IsUp) > -1;
    
    public bool IsPressed => _targets.FindIndex(x => x.IsPressed) > -1;

    private List<IInputMapper> _targets = new ();
    
    public CombineMapper(IEnumerable<IInputMapper> targets)
    {
        _targets.AddRange(targets);
    }
}