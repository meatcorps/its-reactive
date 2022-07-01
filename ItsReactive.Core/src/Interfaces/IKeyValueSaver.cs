namespace ItsReactive.Core.Interfaces;

public interface IKeyValueSaver<TEnum> where TEnum : struct, IConvertible
{
    void SetTarget(IKeyValueDatabase<TEnum> target);
    void Save();
}