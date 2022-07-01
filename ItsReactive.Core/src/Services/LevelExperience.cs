using ItsReactive.Core.Adapters;
using ItsReactive.Core.Background;
using ItsReactive.Core.Helpers;
using ItsReactive.Core.Helpers;
using ItsReactive.Core.Interfaces;
using ItsReactive.Core.Lists;

namespace ItsReactive.Core.Services;

public class LevelExperience: IDisposable
{
    private readonly IBroker<Topic> _broker;
    private readonly List<IDisposable> _subscriptions = new ();
    private readonly IKeyValueDatabaseCollection<StorageType, GameKeyStorage> _collection;

    public int CurrentExp
    {
        get => int.Parse(_collection.GetItemOrSetDefault<string>(StorageType.Persistent, GameKeyStorage.Exp, "0"));
        private set => _collection.SetItem<string>(StorageType.Persistent, GameKeyStorage.Exp, value.ToString());
    } 
    public int Level
    {
        get => int.Parse(_collection.GetItemOrSetDefault<string>(StorageType.Persistent, GameKeyStorage.Level, "1"));
        private set => _collection.SetItem<string>(StorageType.Persistent, GameKeyStorage.Level, value.ToString());
    } 
    public int NextLevelExp
    {
        get => int.Parse(_collection.GetItemOrSetDefault<string>(StorageType.Persistent, GameKeyStorage.NextLevelExp, "1000"));
        private set => _collection.SetItem<string>(StorageType.Persistent, GameKeyStorage.NextLevelExp, value.ToString());
    } 
    
    
    public LevelExperience(IBroker<Topic> broker, IKeyValueDatabaseCollection<StorageType, GameKeyStorage> collection, ILoggerAdapter<LevelExperience> logger)
    {
        _broker = broker;
        _collection = collection;

        _subscriptions.Add(_broker.Subscribe<int>(Topic.AddExp).Subscribe(AddExp));
    }

    private void AddExp(int amount)
    {
        var current = CurrentExp;
        var nextLevel = NextLevelExp;
        
        current += amount;
        if (current >= nextLevel)
        {
            Level += 1;
            NextLevelExp = current + (Level * 200 + 2000);
            _broker.Push(Topic.Dialog, new DialogSettings
            {
                Message = $"You reached level {Level}!\nNext level in {NextLevelExp} EXP! You have {current}",
                Icon = TileSetList.Ok,
                Duration = 5000
            });
        }
        else
        {
            _broker.Push(Topic.Dialog, new DialogSettings
            {
                Message = $"{amount} EXP added!\nNext level in {NextLevelExp} EXP! You need {NextLevelExp - current}",
                Icon = TileSetList.Ok,
                Duration = 5000
            });
        }

        CurrentExp = current;
    }

    public int EXPNeededForNextLevel
    {
        get => NextLevelExp - CurrentExp;
    }

    public void Dispose()
    {
        _subscriptions.ForEach(x => x.Dispose());
        GC.SuppressFinalize(this);
    }
}