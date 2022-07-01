using System.Collections.Generic;
using System.Linq;
using ItsReactive.Core;
using ItsReactive.Core.Adapters;
using ItsReactive.Core.Background;
using ItsReactive.Core.Helpers;
using ItsReactive.Core.Interfaces;
using ItsReactive.Core.Lists;
using ItsReactive.Core.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ItsReactive
{
    public class MainMenu : BaseGame
    {
        protected readonly ILoggerAdapter<MainMenu> _logger;
        private readonly RenderHelper _render;
        private readonly IInputEvent _input;
        private readonly LevelExperience _levelExperience;

        private List<string> _games = new ();
        private int _currentSelected = 0;
        
        private Dictionary<string, SimpleAnimation> _animations = new ();

        public MainMenu(
            IBroker<Topic> broker, 
            IKeyValueDatabaseCollection<StorageType, 
                GameKeyStorage> collection, 
            ILoggerAdapter<MainMenu> logger, 
            RenderHelper render, 
            IInputEvent input,
            LevelExperience levelExperience) : base(broker, collection)
        {
            _logger = logger;
            _render = render;
            _input = input;
            _levelExperience = levelExperience;
            Name = "MainMenu";
        }

        public override void Initialize(GraphicsDeviceManager graphicsDeviceManager)
        {
            
            _games.Clear();
            _games.AddRange(SceneManager.AllScenes.Where(x => x != Name).ToArray());
            
            _animations.Add("Up", new SimpleAnimation(new [] {TileSetList.Up, TileSetList.UpDown}, 400));
            _animations.Add("Down", new SimpleAnimation(new [] {TileSetList.Down, TileSetList.DownDown}, 400));
            _animations.Add("Esc", new SimpleAnimation(new [] {TileSetList.Esc, TileSetList.EscDown}, 400));
            _animations.Add("EnterL", new SimpleAnimation(new [] {TileSetList.EnterL, TileSetList.EnterLDown}, 400));
            _animations.Add("EnterR", new SimpleAnimation(new [] {TileSetList.EnterR, TileSetList.EnterRDown}, 400));
            _animations.Add("UpDown", new SimpleAnimation(3, 200));
            
            Music(MusicList.Menu);
            
            base.Initialize(graphicsDeviceManager);
        }

        public override void Update(GameTime gameTime)
        {
            if (_input.Down.IsPressed && _currentSelected < _games.Count - 1)
            {
                _currentSelected++;
                Effect(SoundEffectList.Ui);
            }
            if (_input.Up.IsPressed && _currentSelected > 0)
            {
                _currentSelected--;
                Effect(SoundEffectList.Ui);
            }
            if (_input.Confirm.IsPressed)
            {
                SceneManager.SwitchTo(_games[_currentSelected]);
                Effect(SoundEffectList.PowerUp);
            }

            foreach (var animation in _animations.Values)
            {
                animation.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.Clear(Color.Black);
            _render.StartWithoutCamera(spriteBatch);
            
            _render.RenderRect(new Rectangle(8,8, 304, 32), Color.Gray);
            _render.RenderTile(TileSetList.PointerAlright, new Vector2(16, 16  + (_animations["UpDown"].Index - 1)), Color.White);
            _render.RenderText("Its reactive!", new Vector2(40, 17 + -(_animations["UpDown"].Index - 1)), Color.White);
            
            var y = 52;
            var number = 0;
            _render.RenderRect(new Rectangle(8,46 + (_currentSelected * 20), 173, 21), Color.DarkOrange);
            foreach (var gameName in _games)
            {
                _render.RenderText(gameName, new Vector2(54, y + 2), number == _currentSelected ? Color.Black : Color.DarkGray, 0.35f, spriteOrder: 1);
                y += 20;
                number++;
            }

            _render.RenderTile(_animations["Up"].Tile, new Vector2(11, 48 + (_currentSelected * 20)), 
                new Color(255,255,255, _currentSelected > 0 ? 255: 40));
            _render.RenderTile(_animations["Down"].Tile, new Vector2(27, 48 + (_currentSelected * 20)), 
                new Color(255,255,255, _currentSelected < _games.Count - 1 ? 255: 40));
            
            _render.RenderTile(_animations["EnterL"].Tile, new Vector2(150, 48 + (_currentSelected * 20)), 
                Color.White);
            _render.RenderTile(_animations["EnterR"].Tile, new Vector2(162, 48 + (_currentSelected * 20)), 
                Color.White);

            _render.RenderRect(new Rectangle(8,160, 304, 24), Color.Gray);
            _render.RenderTile(TileSetList.Ok, new Vector2(16, 163 + (_animations["UpDown"].Index - 1)), Color.White);
            _render.RenderText($"Level {_levelExperience.Level} EXP: {_levelExperience.CurrentExp}\nNext level: {_levelExperience.NextLevelExp} EXP", new Vector2(38, 166), Color.White, 0.3f);
            
            _render.RenderTile(_animations["Esc"].Tile, new Vector2(220, 163), 
                Color.White);
            _render.RenderText("Exit game\nIn game back", new Vector2(240, 166), Color.White, 0.3f);
            
            _render.Stop();
        }

        public override void Dispose()
        {
            _animations.Clear();
            base.Dispose();
        }
    }
}