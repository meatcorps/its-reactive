using ItsReactive.Core.Interfaces;
using ItsReactive.Core.Lists;

namespace ItsReactive.Core.Services;

public class PersistentDatabase : Dictionary<GameKeyStorage, object>, IKeyValueDatabase<GameKeyStorage>
{
    public PersistentDatabase(IKeyValueLoader<GameKeyStorage> loader, IKeyValueSaver<GameKeyStorage> saver)
    {

        loader.GetData(this);
        saver.SetTarget(this);
    }
    
}