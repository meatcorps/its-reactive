using ItsReactive.Core.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using ItsReactive.Core.Interfaces;
using ItsReactive.Core.Lists;
using ItsReactive.Core.Services;

namespace ItsReactive.Core.Background;

public class Music: IBackgroundWorker
{
    private readonly IBroker<Topic> _broker;
    private readonly Dictionary<MusicList, Song> _music = new ();
    private readonly List<IDisposable> _subscriptions = new ();
    private MusicList _wantToPlay = MusicList.Stop;
    
    public Music(IBroker<Topic> broker)
    {
        _broker = broker;
        _subscriptions.Add(broker.Subscribe<MusicList>(Topic.SelectMusic).Subscribe(SwitchMusic));
    }

    private void SwitchMusic(MusicList toSong)
    {
        if (toSong == MusicList.Stop)
        {
            MediaPlayer.Stop();
            return;
        }

        if (!_music.ContainsKey(toSong))
        {
            _wantToPlay = toSong;
            return;
        }

        _wantToPlay = MusicList.Stop;
        MediaPlayer.Play(_music[toSong]);
    }

    public void Initialize(GraphicsDeviceManager graphicsDeviceManager)
    {
        MediaPlayer.IsRepeating = true;
    }

    public void LoadContent(ContentManager contentManager)
    {
        // TODO: Check music list
        _music.Add(MusicList.Menu, contentManager.Load<Song>("ingame"));
        _music.Add(MusicList.Score, contentManager.Load<Song>("score"));
        _music.Add(MusicList.InGame, contentManager.Load<Song>("ingame"));

        if (_wantToPlay != MusicList.Stop)
            SwitchMusic(_wantToPlay);
    }

    public void UnloadContent(ContentManager contentManager)
    {
        
    }

    public void Update(GameTime gameTime)
    { }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
    { }

    public void Dispose()
    {
        foreach (var song in _music.Values)
        {
            song.Dispose();
        }
        
        _subscriptions.ForEach(x => x.Dispose());
    }
    
    public bool AlwaysOn => true;
    public bool UpdateAndDrawLast => false;
}

public enum MusicList
{
    InGame,
    Menu,
    Score,
    Stop
}