namespace ItsReactive.Core.Interfaces;

public interface IKeyValueLoader<TEnum> where TEnum : struct, IConvertible
{
    void GetData(IKeyValueDatabase<TEnum> target);
}