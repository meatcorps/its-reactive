using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using ItsReactive.Core.Adapters;
using ItsReactive.Core.Helpers;
using ItsReactive.Core.Helpers;
using ItsReactive.Core.Interfaces;
using ItsReactive.Core.Lists;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;

namespace ItsReactive.Core.Services;

public class PersistentLoaderAndSaver: IKeyValueLoader<GameKeyStorage>, IKeyValueSaver<GameKeyStorage>, IDisposable
{
    private IKeyValueDatabase<GameKeyStorage>? _target;
    private const string Filename = "session.bda";
    private readonly ILoggerAdapter<PersistentLoaderAndSaver> _logger;

    public PersistentLoaderAndSaver(ILoggerAdapter<PersistentLoaderAndSaver> logger)
    {
        _logger = logger;
    }

    public void GetData(IKeyValueDatabase<GameKeyStorage> target)
    {
        _logger.LogInformation($"Try to load: " + AppContext.BaseDirectory + Filename);
        if (!File.Exists(Filename))
            return;

        try
        {
            var json = Encrypt.Decrypt(File.ReadAllText(AppContext.BaseDirectory + Filename));

            _logger.LogInformation($"Loading json information\n{json}");
            
            var data = JsonSerializer.Deserialize<Dictionary<GameKeyStorage, string>>(json);

            foreach (var key in data.Keys)
            {
                target.Add(key, data[key]);
            }
        }
        catch (FormatException exception)
        {
            File.Move(AppContext.BaseDirectory + Filename,AppContext.BaseDirectory + $"{Filename}_CouldNotLoad_{DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString()}");
        }
    }

    public void SetTarget(IKeyValueDatabase<GameKeyStorage> target)
    {
        _target = target;
    }

    public void Save()
    {
        if (File.Exists(AppContext.BaseDirectory + Filename))
            File.Delete(AppContext.BaseDirectory + Filename);
        
        if (_target is null)
            return;

        var savingDictionary = _target.Keys.ToDictionary(key => key, key => _target[key].ToString());

        var json = JsonSerializer.Serialize(savingDictionary);
        
        File.WriteAllText(AppContext.BaseDirectory + Filename, Encrypt.Encrypt(json));
        
        _logger.LogInformation($"Saving json information\n{json}");
        _logger.LogInformation($"To: " + AppContext.BaseDirectory + Filename);

        if (!File.Exists(AppContext.BaseDirectory + Filename))
            _logger.LogInformation($"Failed to save data! Backup is here: \n" + Encrypt.Encrypt(json));
    }

    private Encryptor<TwofishEngine, Sha1Digest> Encrypt => new Encryptor<TwofishEngine, Sha1Digest>(Encoding.UTF8, Key, MacKey);

    private byte[] Key => Encoding.ASCII.GetBytes("mortiuniseenmortiun");
    private byte[] MacKey => Encoding.ASCII.GetBytes("12345453451215");

    public void Dispose()
    {
        // Save();
    }
}