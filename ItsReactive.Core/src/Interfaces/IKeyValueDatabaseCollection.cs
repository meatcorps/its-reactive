namespace ItsReactive.Core.Interfaces;

public interface IKeyValueDatabaseCollection<in TEnum, TCollectionEnum> 
    where TEnum : struct, IConvertible 
    where TCollectionEnum : struct, IConvertible
{
    IKeyValueDatabaseCollection<TEnum, TCollectionEnum> SetKeyValueDatabase(TEnum group, IKeyValueDatabase<TCollectionEnum> collection);

    void SetItem<T>(TEnum group, TCollectionEnum key, T? data);
    T? GetItem<T>(TEnum group, TCollectionEnum key);
    T GetItemOrSetDefault<T>(TEnum group, TCollectionEnum key, T defaultData);
    void RemoveItem(TEnum group, TCollectionEnum key);
    void Reset(TEnum group);
    IKeyValueDatabase<TCollectionEnum>? Collection(TEnum group);
}