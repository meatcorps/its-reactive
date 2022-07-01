using System;
using System.Collections.Generic;
using ItsReactive.Core.Adapters;
using ItsReactive.Core.Background;
using ItsReactive.Core.Interfaces;
using ItsReactive.Core.Lists;
using ItsReactive.Core.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace ItsReactive
{
    public class MainGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        [CanBeNull] private IScreen _screen;
        private List<IDisposable> _subscriptions = new();
        private readonly IBroker<Topic> _broker;
        private readonly IKeyValueDatabaseCollection<StorageType, GameKeyStorage> _collection;
        private BoxingViewportAdapter _viewport;
        private OrthographicCamera _camera;
        protected readonly ILoggerAdapter<MainGame> _logger;
        private readonly IInputEvent _inputEvent;
        private readonly ISceneManager _sceneManager;
        private IEnumerable<IBackgroundWorker> _backgroundWorkers;

        private const int WindowWidth = 1280;
        private const int WindowHeight = 720;

        public MainGame(
            IBroker<Topic> broker, 
            IKeyValueDatabaseCollection<StorageType, 
                GameKeyStorage> collection, 
            ILoggerAdapter<MainGame> logger, 
            IInputEvent inputEvent, 
            ISceneManager sceneManager, 
            IEnumerable<IBackgroundWorker> backgroundWorkers)
        {
            _broker = broker;
            _collection = collection;
            _logger = logger;
            _inputEvent = inputEvent;
            _sceneManager = sceneManager;
            _backgroundWorkers = backgroundWorkers;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
            IsMouseVisible = true;
            _subscriptions.Add(broker.Subscribe<IScreen>(Topic.SwitchScene).Subscribe(SwitchScene));
            _subscriptions.Add(broker.Subscribe<bool>(Topic.Exit).Subscribe(data => Exit()));
        }

        private void SwitchScene(IScreen scene)
        {
            if (scene is IBaseGame baseGame)
                _logger.LogInformation("Switching to scene {0}", baseGame.Name);
            
            _screen?.UnloadContent(Content);
            _screen?.Dispose();
            
            if (_screen != null)
            {
                foreach (var backgroundWorker in _backgroundWorkers)
                {
                    if (!backgroundWorker.AlwaysOn)
                        backgroundWorker.UnloadContent(Content);
                }
            }
            
            _screen = scene;
            _screen!.Initialize(_graphics);
            _screen!.LoadContent(Content);
            
            foreach (var backgroundWorker in _backgroundWorkers)
            {
                if (backgroundWorker.AlwaysOn) 
                    continue;
                
                backgroundWorker.Initialize(_graphics);
                backgroundWorker.LoadContent(Content);
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
            
            _viewport = new BoxingViewportAdapter(Window, GraphicsDevice, 1024, 576); 
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _camera = new OrthographicCamera(_viewport);
            
            _collection.SetItem(
                StorageType.InMemory,
                GameKeyStorage.Viewport,
                (ViewportAdapter)_viewport);
            
            _collection.SetItem(
                StorageType.InMemory,
                GameKeyStorage.Camera,
                (Camera<Vector2>)_camera);
            
            foreach (var backgroundWorker in _backgroundWorkers)
                backgroundWorker.Initialize(_graphics);

            WindowMode();
        }

        protected override void LoadContent()
        {
            _collection.SetItem(
                StorageType.InMemory, 
                GameKeyStorage.Font, 
                Content.Load<SpriteFont>("main"));
            
            _collection.SetItem(
                StorageType.InMemory, 
                GameKeyStorage.Sprite, 
                Content.Load<Texture2D>("ItsReactive"));
            
            foreach (var backgroundWorker in _backgroundWorkers)
                backgroundWorker.LoadContent(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            _inputEvent.Update();
            
            foreach (var backgroundWorker in _backgroundWorkers)
            {
                if (!backgroundWorker.UpdateAndDrawLast)
                    backgroundWorker.Update(gameTime);
            }
            
            if (_inputEvent.Back.IsPressed && _sceneManager.CurrentScene == typeof(MainMenu))
                _broker.Push(Topic.Exit, true);

            if (_inputEvent.Back.IsPressed && _sceneManager.CurrentScene != typeof(MainMenu))
            {
                _sceneManager.SwitchTo(typeof(MainMenu));
                _broker.Push(Topic.SoundEffect, SoundEffectList.Damage);
            }

            if (_inputEvent.FullScreenToggle)
                ToggleWindowFullScreen();

            _screen?.Update(gameTime);

            foreach (var backgroundWorker in _backgroundWorkers)
            {
                if (backgroundWorker.UpdateAndDrawLast)
                    backgroundWorker.Update(gameTime);
            }
            
            base.Update(gameTime);
        }

        private void ToggleWindowFullScreen()
        {
            _graphics.IsFullScreen = !_graphics.IsFullScreen;
            
            if (_graphics.IsFullScreen)
            {
                _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
            else
            {
                _graphics.PreferredBackBufferHeight = WindowHeight;
                _graphics.PreferredBackBufferWidth = WindowWidth;
            }
            
            _graphics.ApplyChanges();
        }

        private void WindowMode()
        {
            _graphics.PreferredBackBufferHeight = WindowHeight;
            _graphics.PreferredBackBufferWidth = WindowWidth;
            _graphics.ApplyChanges();
        }

        protected override void Draw(GameTime gameTime)
        {
            foreach (var backgroundWorker in _backgroundWorkers)
            {
                if (!backgroundWorker.UpdateAndDrawLast)
                    backgroundWorker.Draw(gameTime, _spriteBatch, GraphicsDevice);
            }
            
            _screen?.Draw(gameTime, _spriteBatch, GraphicsDevice);
            
            foreach (var backgroundWorker in _backgroundWorkers)
            {
                if (backgroundWorker.UpdateAndDrawLast)
                    backgroundWorker.Draw(gameTime, _spriteBatch, GraphicsDevice);
            }
            
            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            _screen?.UnloadContent(Content);
            Content.Unload();
            _screen?.Dispose();
            _subscriptions.ForEach(x => x.Dispose());
            
            foreach (var backgroundWorker in _backgroundWorkers)
                backgroundWorker.Dispose();
            
            base.OnExiting(sender, args);
        }
    }
}