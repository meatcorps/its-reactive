using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Xna.Framework;

namespace ItsReactive.Utilities;

public class TimeLooper : IDisposable
{
    private TimeSpan _totalTime;
    private TimeSpan _startTime;
    private TimeSpan _currentTime;
    private Subject<bool> OnResetSubject = new();
    private Subject<bool> OnFinishSubject = new();
    public IObservable<bool> OnReset => OnResetSubject.AsObservable();
    public IObservable<bool> OnFinish => OnFinishSubject.AsObservable();

    public TimeLooper(TimeSpan totalTime)
    {
        _totalTime = totalTime;
    }

    public void Reset(GameTime gameTime)
    {
        OnResetSubject.OnNext(true);
        _startTime = DateTime.Now.TimeOfDay;//gameTime.TotalGameTime;
    }
    
    public void Update(GameTime gameTime)
    {
        _currentTime = DateTime.Now.TimeOfDay;

        if (CurrentTime <= _totalTime)
            return;
        
        OnFinishSubject.OnNext(true);
        Reset(gameTime);
        
    }
    
    public TimeSpan CurrentTime
        => _currentTime - _startTime;

    public TimeSpan RoundCurrentTimeInMilliSeconds(int round)
    {
        return TimeSpan.FromMilliseconds(
            Math.Round(CurrentTime.TotalMilliseconds / round) * round);
    } 

    public void Dispose()
    {
        OnResetSubject.Dispose();
        OnFinishSubject.Dispose();
        GC.SuppressFinalize(this);
    }
}