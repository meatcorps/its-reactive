using ItsReactive.Core.Background;
using ItsReactive.Core.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using ItsReactive.Core.Helpers;
using ItsReactive.Core.Interfaces;
using ItsReactive.Core.Lists;
using ItsReactive.Core.Services;

namespace ItsReactive.Core;

public abstract class BaseGame: IScreen, IBaseGame, IDisposable
{
    public string Name { get; protected set; }
    public ISceneManager SceneManager { get; private set; }


    protected readonly IKeyValueDatabaseCollection<StorageType, GameKeyStorage> _collection;
    protected readonly IBroker<Topic> _broker;

    protected BaseGame(IBroker<Topic> broker, IKeyValueDatabaseCollection<StorageType, GameKeyStorage> collection)
    {
        _broker = broker;
        _collection = collection;
    }

    public void SetSceneManager(ISceneManager sceneManager)
    {
        SceneManager = sceneManager;
    }

    public virtual void Initialize(GraphicsDeviceManager graphicsDeviceManager)
    {
    }

    public virtual void LoadContent(ContentManager contentManager)
    {
    }
    
    public virtual void UnloadContent(ContentManager contentManager)
    {
    }

    public abstract void Update(GameTime gameTime);

    public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice);

    public virtual void Dispose()
    {
        _collection.Reset(StorageType.OnlyGame);
    }

    protected void Effect(SoundEffectList effect)
    {
        _broker.Push(Topic.SoundEffect, effect);
    }

    protected void Music(MusicList song)
    {
        _broker.Push(Topic.SelectMusic, song);
    }

    protected void ShowPopup(TileSetList icon, string text, double duration = 1000)
    {
        _broker.Push(Topic.Dialog, new DialogSettings
        {
            Message = text,
            Type = DialogType.Popup,
            Icon = icon,
            Duration = duration,
        });
    }
}