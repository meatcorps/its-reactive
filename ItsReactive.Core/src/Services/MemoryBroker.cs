using System.Reactive.Linq;
using System.Reactive.Subjects;
using ItsReactive.Core.Lists;

namespace ItsReactive.Core.Services;

public class MemoryBroker: IBroker<Topic>, IDisposable
{
    private readonly Dictionary<Topic, Dictionary<Type, Subject<object>>> _topicSubjects = new();

    public void Push<T>(Topic to, T? item)
    {
        GetOrCreateSubject<T>(to).OnNext(item);
    }

    public IObservable<T> Subscribe<T>(Topic to)
    {
        return GetOrCreateSubject<T>(to).AsObservable().Cast<T>();
    }

    private Subject<object> GetOrCreateSubject<T>(Topic to)
    {
        if (!_topicSubjects.ContainsKey(to))
            _topicSubjects.Add(to, new Dictionary<Type, Subject<object>>());

        if (!_topicSubjects[to].ContainsKey(typeof(T)))
        {
            _topicSubjects[to].Add(typeof(T), new Subject<object>()); 
        }
        
        return _topicSubjects[to][typeof(T)];
    }

    public void Dispose()
    {
        foreach (var topicDictionaries in _topicSubjects.Values)
        {
            foreach (var subjects in topicDictionaries.Values)
            {
                subjects.Dispose();
            }
        }
        GC.SuppressFinalize(this);
    }
}