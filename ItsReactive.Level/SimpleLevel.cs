using System;
using System.Globalization;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ItsReactive.Core;
using ItsReactive.Core.Helpers;
using ItsReactive.Core.Interfaces;
using ItsReactive.Core.Lists;
using ItsReactive.Core.Services;
using ItsReactive.UI.Components;
using ItsReactive.UI.Data.TimelineUIComponent;
using ItsReactive.Utilities;
using JetBrains.Annotations;
using Microsoft.Reactive.Testing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ItsReactive.Level;

public class SimpleLevel: BaseGame
{
    private RenderHelper _renderHelper;

    private string _levelName = "Simple Delay";
    private Subject<string> _realSubject = new ();
    private TimelineUIComponent _timeline = new ();
    private TimeLooper _timeLooper = new (TimeSpan.FromSeconds(5));
    private List<TimelineItem> _output = new ();

    public SimpleLevel([NotNull] IBroker<Topic> broker, [NotNull] IKeyValueDatabaseCollection<StorageType, GameKeyStorage> collection, RenderHelper renderHelper) : base(broker, collection)
    {
        _renderHelper = renderHelper;
        Name = "SimpleLevel";

        _realSubject
            .Delay(TimeSpan.FromMilliseconds(200))
            .Subscribe(x => 
                _output.Add(new TimelineItem(x, _timeLooper.RoundCurrentTimeInMilliSeconds(20), Color.Green)));
        
    }

    public override void Initialize(GraphicsDeviceManager graphicsDeviceManager)
    {
        base.Initialize(graphicsDeviceManager);
        
        _timeline.Rect = new Rectangle(8, 440, 1008, 128);
        
        _timeline.Rows.Add(new TimelineRow("Input", new []
        {
            new TimelineItem("1", TimeSpan.FromSeconds(1), Color.Gray),
            new TimelineItem("2", TimeSpan.FromSeconds(2), Color.Gray),
            new TimelineItem("3", TimeSpan.FromSeconds(3), Color.Gray),
            new TimelineItem("4", TimeSpan.FromSeconds(4), Color.Gray),
        }));
        _timeline.Rows.Add(new TimelineRow("Expecting", new []
        {
            new TimelineItem("1", TimeSpan.FromSeconds(1.2), Color.Purple),
            new TimelineItem("2", TimeSpan.FromSeconds(2.2), Color.Purple),
            new TimelineItem("3", TimeSpan.FromSeconds(3.2), Color.Purple),
            new TimelineItem("4", TimeSpan.FromSeconds(4.2), Color.Purple),
        }));
        _timeline.Rows.Add(new TimelineRow("Output"));
        _output = _timeline.GetColumns("Output")!;

        _timeLooper.OnReset.Subscribe(x => Reset());
    }

    public override void Update(GameTime gameTime)
    {
        _timeLooper.Update(gameTime);
        _timeline.CurrentTime = _timeLooper.CurrentTime;
    }

    private void Reset()
    {
        _output.Clear();
        foreach (var column in _timeline.GetColumns("Input")!)
        {
            sendWithDelay(column.Position, column.Title);
        }
    }

    private void sendWithDelay(TimeSpan total, string text)
    {
        var subject = new Subject<string>();
        subject
            .Delay(total)
            .Take(1)
            .Subscribe(x => _realSubject.OnNext(x));
        subject.OnNext(text);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
    {
        graphicsDevice.Clear(Color.Black);
        
        _renderHelper.StartWithoutCamera(spriteBatch);
        
        _renderHelper.RenderRect(new Rectangle(8,8, 1008, 32), new Color(0.2f,0.2f,0.2f));
        _renderHelper.RenderText(_levelName, new Vector2(512, 16), Color.Red, scale: 1, align: RenderHelper.TextAlign.Center);

        _timeline.Render(_renderHelper, spriteBatch);
        
        _renderHelper.Stop();
    }

    private void TestObserable()
    {
        
    }

    public override void Dispose()
    {
        base.Dispose();
    }
}