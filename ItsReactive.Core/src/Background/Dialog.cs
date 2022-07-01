using ItsReactive.Core.Helpers;
using ItsReactive.Core.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ItsReactive.Core.Helpers;
using ItsReactive.Core.Interfaces;
using ItsReactive.Core.Lists;
using ItsReactive.Core.Services;

namespace ItsReactive.Core.Background;

public class Dialog: IBackgroundWorker
{
    private readonly IBroker<Topic> _broker;
    private readonly IKeyValueDatabaseCollection<StorageType, GameKeyStorage> _collection;
    private readonly RenderHelper _renderHelper;
    private readonly List<IDisposable> _subscriptions = new ();
    private readonly List<IDisposable> _dialogSubscriptions = new ();
    private DialogSettings? _currentDialog = null;
    private readonly SimpleAnimation _durationCounter;
    private readonly SimpleAnimation _popUpAnimation;
    private readonly IInputEvent _inputEvent;
    private readonly Queue<DialogSettings> _dialogQueue = new ();
    private bool _dialogOpen = false;
    private StateList _currentState;
    
    private enum StateList
    {
        Start,
        Showing,
        Ending
    }
    
    public Dialog(
        IBroker<Topic> broker, 
        IKeyValueDatabaseCollection<StorageType, GameKeyStorage> collection,
        RenderHelper renderHelper, IInputEvent inputEvent)
    {
        _broker = broker;
        _collection = collection;
        _renderHelper = renderHelper;
        _inputEvent = inputEvent;
        _durationCounter = new SimpleAnimation(10, 1000, one: true, @on: false);
        _popUpAnimation = new SimpleAnimation(6, 20, one: true, @on: false);
    }

    public void Initialize(GraphicsDeviceManager graphicsDeviceManager)
    {
        _subscriptions.Add(_broker.Subscribe<DialogSettings>(Topic.Dialog).Subscribe(OpenDialog));
    }

    private void OpenDialog(DialogSettings dialog)
    {
        if (_dialogOpen)
        {
            _dialogQueue.Enqueue(dialog);
            return;
        }
        
        _dialogOpen = true;
        _currentDialog = dialog;
        _currentState = StateList.Start;
        
        if (_currentDialog?.Type != DialogType.Popup) 
            _broker.Push(Topic.Main, "UpdateSceneOff");

        _dialogSubscriptions.Add(_popUpAnimation.OnEnd.Subscribe(index =>
        {
            switch (_currentState)
            {
                case StateList.Start:
                    _currentState = StateList.Showing;
                    break;
                case StateList.Showing:
                    break;
                case StateList.Ending:
                    CloseDialog();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }));
        
        _dialogSubscriptions.Add(_durationCounter.OnEnd.Subscribe(index => StartClosingDialog()));
        
        _popUpAnimation.Reset();
        
        if (_currentDialog?.Duration == 0) return;
        _durationCounter.Reset(dialog.Duration / 10);
    }

    private void StartClosingDialog()
    {
        if (_currentState != StateList.Showing)
            return;
        
        _currentState = StateList.Ending;
        _popUpAnimation.Reset();
    }

    private void CloseDialog()
    {
        _dialogOpen = false;
        
        if (_currentDialog?.Type != DialogType.Popup) 
            _broker.Push(Topic.Main, "UpdateSceneOn");
        
        _dialogSubscriptions.ForEach(x => x.Dispose());
        _currentDialog?.OnClosing.OnNext("Done");
        
        if (_dialogQueue.TryDequeue(out var newDialog))
        {
            OpenDialog(newDialog);
        }
    }
    
    public void LoadContent(ContentManager contentManager)
    { }

    public void UnloadContent(ContentManager contentManager)
    { }

    public void Update(GameTime gameTime)
    {
        if (!_dialogOpen)
            return;

        _durationCounter.Update(gameTime);
        _popUpAnimation.Update(gameTime);
        
        if (_inputEvent.Confirm.IsPressed)
            StartClosingDialog();
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
    {
        if (!_dialogOpen)
            return;
        
        var yPosition = 150;

        if (_currentState == StateList.Start)
            yPosition += 32 - _popUpAnimation.Index * 5;
        
        if (_currentState == StateList.Ending)
            yPosition += _popUpAnimation.Index * 5;
        
        _renderHelper.StartWithoutCamera(spriteBatch);
        _renderHelper.RenderRect(new Rectangle(8, yPosition, 304, 60), new Color(0, 255, 255, 20), spriteOrder: 0);
        _renderHelper.RenderTile(_currentDialog.Value.Icon, new Vector2(16, yPosition + 8), Color.White);
        _renderHelper.RenderText(_currentDialog?.Message!, new Vector2(38, yPosition + 8), Color.Black, scale: 0.4f, spriteOrder: 0);
        _renderHelper.Stop();
    }
    
    public void Dispose()
    {
        _subscriptions.ForEach(x => x.Dispose());
    }

    public bool AlwaysOn => true;
    public bool UpdateAndDrawLast => true;
}