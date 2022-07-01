using ItsReactive.Core.Background;
using ItsReactive.Core.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using ItsReactive.Core.Helpers;
using ItsReactive.Core.Interfaces;
using ItsReactive.Core.Lists;
using ItsReactive.Core.Services;

namespace ItsReactive.Core;

public abstract class BaseGameWithIntroOutro : BaseGame
{
    private readonly IKeyValueDatabaseCollection<StorageType, GameKeyStorage> _collection;
    protected readonly RenderHelper Render;
    protected readonly IInputEvent InputEvent;
    private readonly IHighScore _highScore;
    private Dictionary<string, SimpleAnimation> _animations = new ();
    private List<IDisposable> _subscriptionList = new();
    protected Camera<Vector2> Camera;
    private int _totalExp = 0;
    private bool _newHighScore = false;
    protected int TotalExp => _totalExp;

    protected string GameOverTitle = "Game Over"; 
    protected string GameOverString = ""; 
    protected string IntroDescription1 = ""; 
    protected string IntroDescription2 = ""; 
    
    protected enum GameStateList
    {
        Intro,
        Running,
        GameOver
    }
    protected GameStateList GameState = GameStateList.Intro;
    protected Random Random { get; private set; }

    public BaseGameWithIntroOutro(IBroker<Topic> broker, 
        IKeyValueDatabaseCollection<StorageType, GameKeyStorage> collection,
        RenderHelper render,
        IInputEvent inputEvent,
        IHighScore highScore
        ) : base(broker, collection)
    {
        _collection = collection;
        Render = render;
        InputEvent = inputEvent;
        _highScore = highScore;
    }
    
    public override void Initialize(GraphicsDeviceManager graphicsDeviceManager)
    {
        Music(MusicList.InGame);
        Camera = _collection.GetItem<Camera<Vector2>>(StorageType.InMemory, GameKeyStorage.Camera)!;
        Camera.Position = Vector2.Zero;
        GameState = GameStateList.Intro;
        _animations.Add("CountDown", new SimpleAnimation(3, 2000, one: true, on: true));
        _animations.Add("Esc", new SimpleAnimation(new [] {TileSetList.Esc, TileSetList.EscDown}, 400));
        _animations.Add("SpaceL", new SimpleAnimation(new [] {TileSetList.SpaceBarL, TileSetList.SpaceBarLDown}, 400));
        _animations.Add("SpaceM", new SimpleAnimation(new [] {TileSetList.SpaceBarM, TileSetList.SpaceBarMDown}, 400));
        _animations.Add("SpaceR", new SimpleAnimation(new [] {TileSetList.SpaceBarR, TileSetList.SpaceBarRDown}, 400));
        _animations.Add("Up", new SimpleAnimation(new [] {TileSetList.Up, TileSetList.UpDown}, 400));
        _animations.Add("Down", new SimpleAnimation(new [] {TileSetList.Down, TileSetList.DownDown}, 400));
        _animations.Add("Left", new SimpleAnimation(new [] {TileSetList.Left, TileSetList.LeftDown}, 400));
        _animations.Add("Right", new SimpleAnimation(new [] {TileSetList.Right, TileSetList.RightDown}, 400));
        _subscriptionList.Add(_animations["CountDown"].OnEnd.Subscribe(index => StartGame()));
        _subscriptionList.Add(_animations["CountDown"].OnTrigger.Subscribe(index => Effect(SoundEffectList.Ui)));
        Random = new Random();
        _newHighScore = false;
        _totalExp = 0;
        base.Initialize(graphicsDeviceManager);
    }
    
    protected void AddSubscription(IDisposable subscription)
    {
        _subscriptionList.Add(subscription);
    }

    protected void AddAnimation(string name, SimpleAnimation animation)
    {
        _animations.Add(name, animation);
    }
    
    public SimpleAnimation GetAnimation(string name)
    {
        return _animations[name];
    }
    
    protected virtual void StartGame()
    {
        GameState = GameStateList.Running;
        Effect(SoundEffectList.PowerUp);
    }
    
    public override void Update(GameTime gameTime)
    {
        foreach (var animation in _animations)
        {
            animation.Value.Update(gameTime);
        }
        
        switch (GameState)
        {
            case GameStateList.Intro:
                break;
            case GameStateList.Running:
                UpdateGame(gameTime);
                break;
            case GameStateList.GameOver:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected virtual void UpdateGame(GameTime gameTime)
    {

    }

    protected void AddExp(int amount, bool showMessage = true)
    {
        _totalExp += amount;
        
        if (showMessage)
            ShowPopup(TileSetList.PointerAlright, $"{amount} exp earned!", 2000);
    }
    
    protected virtual void GameOver(int finalScore)
    {
        Effect(SoundEffectList.Damage);
        Music(MusicList.Score);

        if (finalScore > _highScore.Get(Name))
        {
            _highScore.Set(Name, finalScore);
            _newHighScore = true;
        }

        if (_totalExp > 0)
            _broker.Push(Topic.AddExp, _totalExp);
        GameState = GameStateList.GameOver;
    }
    
    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
    {
        graphicsDevice.Clear(Color.Black);

        switch (GameState)
        {
            case GameStateList.Intro:
                DrawIntro(spriteBatch);
                break;
            case GameStateList.Running:
                DrawGame(spriteBatch);
                break;
            case GameStateList.GameOver:
                DrawGameOver(spriteBatch);
                break;
        }
        
    }
    
    protected virtual void DrawIntro(SpriteBatch spriteBatch)
    {
        Render.StartWithoutCamera(spriteBatch);
        Render.RenderText(Name, new Vector2(160, 40), Color.Green, align: RenderHelper.TextAlign.Center);
        Render.RenderText((3 - _animations["CountDown"].Index).ToString() + " ...", new Vector2(160, 80), Color.White, scale: 1f, align: RenderHelper.TextAlign.Center);
        Render.RenderText(IntroDescription1, new Vector2(160, 110), Color.Gray, scale: 0.3f, align: RenderHelper.TextAlign.Center);
        Render.RenderText(IntroDescription2, new Vector2(160, 120), Color.Gray, scale: 0.3f, align: RenderHelper.TextAlign.Center);
        if (_highScore.Get(Name) > 0)
        {
            Render.RenderText($"Try to beat {_highScore.Get(Name)}!", new Vector2(160, 130), Color.Purple, scale: 0.3f, align: RenderHelper.TextAlign.Center);
        }
        Render.Stop();
    }

    protected virtual void DrawGame(SpriteBatch spriteBatch)
    {

    }
    
    protected virtual void DrawGameOver(SpriteBatch spriteBatch)
    {
        Render.StartWithoutCamera(spriteBatch);
        Render.RenderText(GameOverTitle, new Vector2(160, 40), Color.Green, align: RenderHelper.TextAlign.Center);
        Render.RenderText(GameOverString, new Vector2(160, 80), Color.White, scale: 1f, align: RenderHelper.TextAlign.Center);
        Render.RenderText($"You gain {_totalExp} EXP!", new Vector2(160, 120), Color.Gray, scale: 0.3f, align: RenderHelper.TextAlign.Center);
        Render.RenderText($"New high score!", new Vector2(160, 130), Color.Purple, scale: 0.3f, align: RenderHelper.TextAlign.Center);
        Render.RenderTile(_animations["Esc"].Tile, new Vector2(235, 160), Color.White);
        Render.RenderText($"To continue", new Vector2(255, 165), Color.White, scale: 0.3f, align: RenderHelper.TextAlign.Left);
        Render.Stop();
    }
    
    public override void Dispose()
    {
        _subscriptionList.ForEach(x => x.Dispose());
        _animations.Clear();
        base.Dispose();
    }
}