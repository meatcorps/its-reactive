using ItsReactive.Core.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ItsReactive.Core.Interfaces;
using ItsReactive.Core.Lists;
using ItsReactive.Core.Services;

namespace ItsReactive.Core.Background;

public class Effect: IBackgroundWorker
{
    private readonly IBroker<Topic> _broker;
    private readonly Dictionary<SoundEffectList, SoundEffect> _effects = new ();
    private readonly List<IDisposable> _subscriptions = new ();
    
    public Effect(IBroker<Topic> broker)
    {
        _broker = broker;
        _subscriptions.Add(broker.Subscribe<SoundEffectList>(Topic.SoundEffect).Subscribe(PlayEffect));
    }

    private void PlayEffect(SoundEffectList effect)
    {
        if (!_effects.ContainsKey(effect))
            return;

        _effects[effect].Play();
    }
    
    public void Initialize(GraphicsDeviceManager graphicsDeviceManager)
    { }

    public void LoadContent(ContentManager contentManager)
    {
        _effects.Add(SoundEffectList.Damage, contentManager.Load<SoundEffect>("Damage01"));
        _effects.Add(SoundEffectList.Ground, contentManager.Load<SoundEffect>("Ground"));
        _effects.Add(SoundEffectList.Jump, contentManager.Load<SoundEffect>("Jump"));
        _effects.Add(SoundEffectList.Ui, contentManager.Load<SoundEffect>("UI01"));
        _effects.Add(SoundEffectList.Walk, contentManager.Load<SoundEffect>("Walk"));
        _effects.Add(SoundEffectList.PowerUp, contentManager.Load<SoundEffect>("PowerUp02"));
        _effects.Add(SoundEffectList.Beep1, contentManager.Load<SoundEffect>("Beep1"));
        _effects.Add(SoundEffectList.Beep2, contentManager.Load<SoundEffect>("Beep2"));
        _effects.Add(SoundEffectList.Beep3, contentManager.Load<SoundEffect>("Beep3"));
        _effects.Add(SoundEffectList.Beep4, contentManager.Load<SoundEffect>("Beep4"));
    }

    public void UnloadContent(ContentManager contentManager)
    { }

    public void Update(GameTime gameTime)
    { }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
    { }

    public void Dispose()
    {
        foreach (var effect in _effects.Values)
        {
            effect.Dispose();
        }
        
        _subscriptions.ForEach(x => x.Dispose());
    }
    
    public bool AlwaysOn => true;
    public bool UpdateAndDrawLast => false;
}

public enum SoundEffectList
{
    Damage,
    Ground,
    Jump,
    PowerUp,
    Ui,
    Walk,
    Beep1,
    Beep2,
    Beep3,
    Beep4,
}