namespace ItsReactive.Core.Interfaces;

public interface IKeyValueDatabase<TEnum>: IDictionary<TEnum, object> where TEnum : struct, IConvertible
{
    
}