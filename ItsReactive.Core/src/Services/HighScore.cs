using ItsReactive.Core.Interfaces;
using ItsReactive.Core.Lists;

namespace ItsReactive.Core.Services;

public class HighScore : IHighScore
{
    private readonly IKeyValueDatabaseCollection<StorageType, GameKeyStorage> _collection;
    private Dictionary<string, int> _highScores = new ();

    public HighScore(IKeyValueDatabaseCollection<StorageType, GameKeyStorage> collection)
    {
        _collection = collection;
        LoadData();
    }

    private void LoadData()
    {
        var extractedData = 
            _collection
            .GetItemOrSetDefault(StorageType.Persistent, GameKeyStorage.HighScore, "")
            .Split('|');
        
        _highScores.Clear();
        
        for (var i = 0; i < extractedData.Length / 2; i++)
            _highScores.Add(extractedData[i * 2], int.Parse(extractedData[i * 2 + 1]));
    }

    public int Get(string gameName) =>
        _highScores.TryGetValue(gameName, out var highScore) ? highScore : 0;

    public void Set(string gameName, int score)
    {
        if (Get(gameName) > score)
            return;

        if (!_highScores.ContainsKey(gameName))
            _highScores.Add(gameName, score);
        else
            _highScores[gameName] = score;

        _collection.SetItem(StorageType.Persistent, GameKeyStorage.HighScore, ToString());
    }

    public override string ToString()
    {
        var toParse = new List<string>();

        foreach (var item in _highScores)
        {
            toParse.Add(item.Key);
            toParse.Add(item.Value.ToString());
        }
        
        return string.Join('|', toParse);
    }
}