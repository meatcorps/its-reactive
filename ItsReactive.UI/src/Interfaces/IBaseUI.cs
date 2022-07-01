using ItsReactive.Core.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ItsReactive.UI.Interfaces;

public interface IBaseUI
{
    Rectangle Rect { get; }
    Vector2 Position { get; }

    void Update(GameTime gameTime);
    void Render(RenderHelper renderHelper, SpriteBatch spriteBatch);
}