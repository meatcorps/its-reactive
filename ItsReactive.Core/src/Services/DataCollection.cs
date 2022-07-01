using ItsReactive.Core.Interfaces;
using ItsReactive.Core.Lists;

namespace ItsReactive.Core.Services;

public class DataCollection : IKeyValueDatabaseCollection<StorageType, GameKeyStorage>
{
    private readonly Dictionary<StorageType, IKeyValueDatabase<GameKeyStorage>?> _collection = new();

    public IKeyValueDatabaseCollection<StorageType, GameKeyStorage> SetKeyValueDatabase(StorageType group, IKeyValueDatabase<GameKeyStorage>? collection)
    {
        _collection.Add(group, collection);
        return this;
    }

    public void SetItem<T>(StorageType group, GameKeyStorage key, T data)
    {
        var collection = Collection(group);
        if (collection == null || data == null) return;
        if (collection.ContainsKey(key))
        {
            collection[key] = data;
            return;
        } 
        collection.Add(key, data);
    }

    public T GetItem<T>(StorageType group, GameKeyStorage key)
    {
        var collection = Collection(group);
        return (T)collection?[key]!;
    }

    public T GetItemOrSetDefault<T>(StorageType group, GameKeyStorage key, T defaultData)
    {
        var collection = Collection(group);
        if (collection == null) return defaultData;
        if (collection.ContainsKey(key))
        {
            return (T)collection[key];
        } 
        collection.Add(key, defaultData);
        return defaultData;
    }

    public void RemoveItem(StorageType group, GameKeyStorage key)
    {
        var collection = Collection(group);
        collection?.Remove(key);
    }

    public void Reset(StorageType group)
    {
        var collection = Collection(group);
        collection?.Clear();
    }

    public IKeyValueDatabase<GameKeyStorage>? Collection(StorageType group)
    {
        return _collection.ContainsKey(group) ? _collection[group] : null;
    }
}