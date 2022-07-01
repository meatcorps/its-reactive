using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Xna.Framework;
using ItsReactive.Core.Helpers;

namespace ItsReactive.Core.Helpers;

public class SimpleAnimation : IDisposable
{
    private readonly TileSetList[]? _list;
    private int _max;
    private readonly bool _pingPong = false;
    private readonly bool _one;
    private readonly Subject<int> _subjectStart = new ();
    private readonly Subject<int> _subjectTrigger = new ();
    private readonly Subject<int> _subjectEnd = new ();
    private double _speed;
    private bool _reverse = false;
    private bool _on = true;
    private double _currentTimer;
    private int _index;
    
    public TileSetList Tile => _list![_index];
    public int Index => _index;
    public IObservable<int> OnStart => _subjectStart.AsObservable();
    public IObservable<int> OnTrigger => _subjectTrigger.AsObservable();
    public IObservable<int> OnEnd => _subjectEnd.AsObservable();
    
    public SimpleAnimation(TileSetList[] list, float speed, bool pingPong = false, bool one = false, bool on = true)
    {
        _list = list;
        _max = list.Length;
        _speed = speed;
        _currentTimer = 0;
        _pingPong = pingPong;
        _one = one;
        _on = on;
    }
    public SimpleAnimation(int max, float speed, bool pingPong = false, bool one = false, bool on = true)
    {
        _max = max;
        _speed = speed;
        _currentTimer = 0;
        _pingPong = pingPong;
        _one = one;
        _on = on;
    }

    public void Reset(double speed = -1, int max = -1)
    {
        if (speed > 0)
            _speed = speed;
        
        if (max > 0)
            _max = max;
        
        _reverse = false;
        _index = 0;
        _subjectStart.OnNext(0);
        _on = true;
    }
    
    public void Update(GameTime gameTime)
    {
        if (!_on)
        {
            _currentTimer = gameTime.TotalGameTime.TotalMilliseconds;
            return;
        }

        var totalTime = gameTime.TotalGameTime.TotalMilliseconds - _currentTimer;
        if (totalTime < _speed)
            return;

        _currentTimer = gameTime.TotalGameTime.TotalMilliseconds;
        
        if (_pingPong)
            DoPingPong();
        else
            DoCounter();
        
        _subjectTrigger.OnNext(_index);
    }

    private void DoPingPong()
    {
        _index += _reverse ? -1 : 1;
        
        if ((_reverse || _index != _max) && (!_reverse || _index != -1)) 
            return;
        
        if (_one && _reverse)
            _index = 0;
        else
            _index = _reverse ? _max - 1 : 0;
            
        if (_reverse)
            Finish();
    }

    private void DoCounter()
    {
        _index++;
        
        if (_index != _max) 
            return;
        
        _index = _one ? _max - 1 : 0;
        Finish();
    }

    private void Finish()
    {
        if (_one)
            _on = false;
        
        _subjectEnd.OnNext(_index);
    }

    public void Dispose()
    {
        _subjectStart.Dispose();
        _subjectTrigger.Dispose();
        _subjectEnd.Dispose();
        GC.SuppressFinalize(this);
    }
}