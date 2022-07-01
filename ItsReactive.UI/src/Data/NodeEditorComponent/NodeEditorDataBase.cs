using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Xna.Framework;

namespace ItsReactive.UI.Data.NodeEditorComponent;

public abstract class NodeEditorDataBase : IDisposable
{
    public string Name { get; set; }
    public Guid Id { get; }

    private Rectangle? _position;

    public Rectangle Position
    {
        get => _position.Value;
        set
        {
            if (!_position.HasValue && _position.Value.Equals(value))
            {
                _position = value;
                return;
            }

            var oldValue = _position.Value;
            _position = value;
            
            if (!_onPositionChangeSubject.IsDisposed)
                _onPositionChangeSubject.OnNext((from: oldValue, to: value));
        }
    }

    private Subject<(Rectangle from, Rectangle to)> _onPositionChangeSubject = new ();
    public IObservable<(Rectangle from, Rectangle to)> OnPositionChange => _onPositionChangeSubject.AsObservable();

    private Subject<Unit> _onDisposedSubject = new ();
    public IObservable<Unit> OnDisposed => _onDisposedSubject.AsObservable();

    protected NodeEditorDataBase(string name, Rectangle position)
    {
        Name = name;
        Position = position;
        Id = Guid.NewGuid();
    }

    protected virtual void OnDispose()
    {
    }

    public void Dispose()
    {
        _onDisposedSubject.OnNext(Unit.Default);
        _onPositionChangeSubject.OnCompleted();
        OnDispose();
        _onPositionChangeSubject.Dispose();
        _onDisposedSubject.Dispose();
        GC.SuppressFinalize(this);
    }
}