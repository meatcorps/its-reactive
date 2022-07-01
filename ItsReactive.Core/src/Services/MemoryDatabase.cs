using ItsReactive.Core.Interfaces;
using ItsReactive.Core.Lists;

namespace ItsReactive.Core.Services;

public class MemoryDatabase : Dictionary<GameKeyStorage, object>, IKeyValueDatabase<GameKeyStorage>
{
}