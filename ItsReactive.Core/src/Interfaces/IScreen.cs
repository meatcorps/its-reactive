using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ItsReactive.Core.Interfaces;

public interface IScreen
{
    public void Dispose();

    public void Initialize(GraphicsDeviceManager graphicsDeviceManager);

    public void LoadContent(ContentManager contentManager);

    public void UnloadContent(ContentManager contentManager);

    public void Update(GameTime gameTime);

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice);
}