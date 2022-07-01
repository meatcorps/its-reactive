namespace ItsReactive.Core.Services;

public interface IBroker<TEnum> where TEnum : struct, IConvertible
{
    public void Push<T>(TEnum to, T? item);
    public IObservable<T> Subscribe<T>(TEnum to);
}