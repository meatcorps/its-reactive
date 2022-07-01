using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using ItsReactive.Core.Helpers;
using ItsReactive.Core.Interfaces;
using ItsReactive.Core.Lists;

namespace ItsReactive.Core.Helpers;

public class RenderHelper
{
    private readonly IKeyValueDatabaseCollection<StorageType, GameKeyStorage> _collection;

    public RenderHelper(IKeyValueDatabaseCollection<StorageType, GameKeyStorage> collection)
    {
        _collection = collection;
    }

    public SpriteFont Font => _collection.GetItem<SpriteFont>(StorageType.InMemory, GameKeyStorage.Font)!;
    public Texture2D TileSet => _collection.GetItem<Texture2D>(StorageType.InMemory, GameKeyStorage.Sprite)!;
    public Camera<Vector2> Camera => _collection.GetItem<Camera<Vector2>>(StorageType.InMemory, GameKeyStorage.Camera)!;

    public ViewportAdapter Viewport =>
        _collection.GetItem<ViewportAdapter>(StorageType.InMemory, GameKeyStorage.Viewport)!;

    public static int TileWidth => 16;
    public static int TileHeight => 16;

    private SpriteBatch _spriteBatch;

    public void StartWithoutCamera(SpriteBatch spriteBatch)
    {
        _spriteBatch = spriteBatch;
        spriteBatch.Begin(transformMatrix: Viewport.GetScaleMatrix()); //, samplerState: SamplerState.PointClamp);
    }

    public void StartWithCamera(SpriteBatch spriteBatch)
    {
        _spriteBatch = spriteBatch;
        spriteBatch.Begin(transformMatrix: Camera.GetViewMatrix()); //, samplerState: SamplerState.PointClamp);
    }

    public void Stop()
    {
        _spriteBatch.End();
    }

    public void RenderTile(Vector2 gridPosition, Vector2 position, Color color, float rotation = 0, int spriteOrder = 0,
        bool flipped = false)
    {
        var (x, y) = gridPosition;
        var sourceRect = new Rectangle((int) x * TileWidth, (int) y * TileHeight, TileWidth, TileHeight);
        var destinationRect = new Rectangle((int) position.X + (TileWidth / 2), (int) position.Y + (TileHeight / 2), TileWidth, TileHeight);

        _spriteBatch.Draw(
            TileSet,
            destinationRect,
            sourceRect,
            color,
            (float) (Math.PI / 180f) * rotation,
            new Vector2(TileWidth / 2, TileHeight / 2),
            flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
            spriteOrder);
    }

    public void RenderTile(TileSetList gridPosition, Vector2 position, Color color, float rotation = 0, int spriteOrder = 0,
        bool flipped = false)
    {
        RenderTile(TileSetMap.GetTilePosition(gridPosition), position, color, rotation, spriteOrder, flipped);
    }

    public void RenderText(string text, Vector2 position, Color color, float scale = 1, float rotation = 0, int spriteOrder = 0, TextAlign align = TextAlign.Left)
    {
        var origin = Vector2.Zero;

        var font = Font;

        if (align != TextAlign.Left)
        {
            var size = font.MeasureString(text);
            origin.X = (align == TextAlign.Center ? size.X / 2 : size.X);
        }

        _spriteBatch.DrawString(
            font, 
            text, 
            position, 
            color,
            (float) (Math.PI / 180f) * rotation,
            origin, 
            new Vector2(scale, scale),
            SpriteEffects.None,
            spriteOrder
            );
    }
    
    public void RenderCircle(Vector2 position, Color color, int spriteOrder = 0)
    {
        RenderTile(new Vector2(2, 3), position, color, spriteOrder: spriteOrder);
    }
    
    public void RenderRect(Rectangle rectangle, Color color, int spriteOrder = 0)
    {
        const int offsetX = 0;
        const int offsetY = 48;

        RenderRectWithOffset(offsetX, offsetY, rectangle, color, spriteOrder);
    }
    
    public void RenderRectBorder(Rectangle rectangle, Color color, int spriteOrder = 0)
    {
        const int offsetX = 16;
        const int offsetY = 48;

        RenderRectWithOffset(offsetX, offsetY, rectangle, color, spriteOrder);
    }

    private void RenderRectWithOffset(int offsetX, int offsetY, Rectangle rectangle, Color color, int spriteOrder = 0)
    {
        var leftTopCornerSource = new Rectangle(offsetX, offsetY, 4, 4);
        var rightTopCornerSource = new Rectangle(offsetX + 12, offsetY, 4, 4);
        var rightBottomCornerSource = new Rectangle(offsetX + 12, offsetY + 12, 4, 4);
        var leftBottomCornerSource = new Rectangle(offsetX, offsetY + 12, 4, 4);
        var topSource = new Rectangle(offsetX + 4, offsetY, 8, 4);
        var bottomSource = new Rectangle(offsetX + 4, offsetY + 12, 8, 4);
        var leftSource = new Rectangle(offsetX, offsetY + 4, 4, 8);
        var rightSource = new Rectangle(offsetX + 12, offsetY + 4, 4, 8);
        var centerSource = new Rectangle(offsetX + 4, offsetY + 4, 8, 8);
        
        _spriteBatch.Draw(
            TileSet, 
            new Rectangle(rectangle.Left, rectangle.Top, 4, 4), 
            leftTopCornerSource, 
            color, 
            0,
            Vector2.Zero, 
            SpriteEffects.None,
            spriteOrder);
        
        _spriteBatch.Draw(
            TileSet, 
            new Rectangle(rectangle.Right - 4, rectangle.Top, 4, 4), 
            rightTopCornerSource, 
            color, 
            0,
            Vector2.Zero, 
            SpriteEffects.None,
            spriteOrder);
        
        _spriteBatch.Draw(
            TileSet, 
            new Rectangle(rectangle.Right - 4, rectangle.Bottom - 4, 4, 4), 
            rightBottomCornerSource, 
            color, 
            0,
            Vector2.Zero, 
            SpriteEffects.None,
            spriteOrder);
        
        _spriteBatch.Draw(
            TileSet, 
            new Rectangle(rectangle.Left, rectangle.Bottom - 4, 4, 4), 
            leftBottomCornerSource, 
            color, 
            0,
            Vector2.Zero, 
            SpriteEffects.None,
            spriteOrder); 
        
        _spriteBatch.Draw(
            TileSet, 
            new Rectangle(rectangle.Left + 4, rectangle.Top, rectangle.Width - 8, 4), 
            topSource, 
            color, 
            0,
            Vector2.Zero, 
            SpriteEffects.None,
            spriteOrder);
        
        _spriteBatch.Draw(
            TileSet, 
            new Rectangle(rectangle.Left + 4, rectangle.Bottom - 4, rectangle.Width - 8, 4), 
            bottomSource, 
            color, 
            0,
            Vector2.Zero, 
            SpriteEffects.None,
            spriteOrder);
        
        _spriteBatch.Draw(
            TileSet, 
            new Rectangle(rectangle.Left, rectangle.Top + 4, 4, rectangle.Height - 8), 
            leftSource, 
            color, 
            0,
            Vector2.Zero, 
            SpriteEffects.None,
            spriteOrder);
        
        _spriteBatch.Draw(
            TileSet, 
            new Rectangle(rectangle.Right - 4, rectangle.Top + 4, 4, rectangle.Height - 8), 
            rightSource, 
            color, 
            0,
            Vector2.Zero, 
            SpriteEffects.None,
            spriteOrder);
        
        _spriteBatch.Draw(
            TileSet, 
            new Rectangle(rectangle.Left + 4, rectangle.Top + 4, rectangle.Width - 8, rectangle.Height - 8), 
            centerSource, 
            color, 
            0,
            Vector2.Zero, 
            SpriteEffects.None,
            spriteOrder);
    }

    public enum TextAlign
    {
        Left,
        Center,
        Right
    } 
}