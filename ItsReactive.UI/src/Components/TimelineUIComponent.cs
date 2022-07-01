using System.Globalization;
using ItsReactive.Core.Helpers;
using ItsReactive.UI.Data.TimelineUIComponent;
using ItsReactive.UI.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace ItsReactive.UI.Components;

public class TimelineUIComponent: IBaseUI
{
    public Rectangle Rect { get; set; }
    public Vector2 Position => new (Rect.X, Rect.Y);
    public TimeSpan TotalTime = TimeSpan.FromSeconds(5);
    public TimeSpan CurrentTime = TimeSpan.Zero;
    public int Padding = 4;
    public float Sections = 20;
    public int RowHeight = 20;
    public readonly List<TimelineRow> Rows = new ();

    public List<TimelineItem>? GetColumns(string title)
        => Rows.Find(x => x.Title == title)?.Items;

    public void Update(GameTime gameTime)
    {
    }

    public void Render(RenderHelper renderHelper, SpriteBatch spriteBatch)
    {
        RenderBackground(renderHelper);
        RenderSections(renderHelper, spriteBatch);
        RenderItems(renderHelper);
        RenderCurrentTime(renderHelper, spriteBatch);
    }

    private void RenderCurrentTime(RenderHelper renderHelper, SpriteBatch spriteBatch)
    {
        renderHelper.RenderText(Math.Round(CurrentTime.TotalSeconds, 1).ToString(),
            new Vector2(getXPositionFromMilli(CurrentTime.TotalMilliseconds), Rect.Y + Padding + 1),
            new Color(0.3f, 0.7f, 0.7f), scale: 0.4f, align: RenderHelper.TextAlign.Center);
        spriteBatch.DrawLine(getXPositionFromMilli(CurrentTime.TotalMilliseconds), Rect.Y + Padding + 1,
            (int) getXPositionFromMilli(CurrentTime.TotalMilliseconds), Rect.Bottom - Padding, new Color(0.3f, 0.7f, 0.7f));
    }

    private void RenderItems(RenderHelper renderHelper)
    {
        for (var i = 0; i < Rows.Count; i++)
        {
            var targetRow = Rows[i];
            var posY = Rect.Y + Padding + 26 + i * RowHeight;

            foreach (var column in targetRow.Items)
            {
                renderHelper.RenderCircle(new Vector2(getXPositionFromTimeSpan(column.Position) - Padding * 2, posY),
                    column.Color);
                renderHelper.RenderText(column.Title,
                    new Vector2(getXPositionFromTimeSpan(column.Position) - Padding, posY + 5), Color.White, 0.5f);
                renderHelper.RenderText(column.Position.TotalSeconds + "ms",
                    new Vector2(getXPositionFromTimeSpan(column.Position) + Padding * 2 + 2, posY + 5), Color.Gray, 0.4f);
            }
        }
    }

    private void RenderSections(RenderHelper renderHelper, SpriteBatch spriteBatch)
    {
        for (var i = 0; i <= Sections; i++)
        {
            var totalWidth = (InnerWidth / Sections);
            var xPos = Rect.X + Padding + i * totalWidth;
            var label = MathF.Round(((float) TotalTime.TotalMilliseconds / Sections * i) / InnerWidth, 1)
                .ToString(CultureInfo.CurrentCulture) + "s";
            renderHelper.RenderText(label.ToString(), new Vector2(xPos, Rect.Y + Padding + 1), Color.White, scale: 0.4f,
                align: RenderHelper.TextAlign.Center);
            if (i < Sections)
            {
                renderHelper.RenderRect(
                    new Rectangle((int) xPos, Rect.Y + Padding + 16, (int) totalWidth / 2,
                        Rect.Height - Padding * 2 - RowHeight),
                    new Color(0.1f, 0.1f, 0.1f));
                renderHelper.RenderRect(
                    new Rectangle((int) xPos + (int) totalWidth / 2, Rect.Y + Padding + 16, (int) totalWidth / 2,
                        Rect.Height - Padding * 2 - RowHeight),
                    new Color(0.15f, 0.15f, 0.15f));
            }

            spriteBatch.DrawLine((int) xPos, Rect.Y + Padding + 11, (int) xPos, Rect.Bottom - Padding - 4,
                new Color(0.3f, 0.3f, 0.3f));
            spriteBatch.DrawLine((int) xPos + 1, Rect.Y + Padding + 11, (int) xPos + 1, Rect.Bottom - Padding - 4,
                new Color(0.3f, 0.3f, 0.3f));
        }
    }

    private void RenderBackground(RenderHelper renderHelper)
    {
        renderHelper.RenderRect(Rect, new Color(0.2f, 0.2f, 0.2f));
    }

    private int getXPositionFromMilli(int milliseconds)
    {
        var result = (Rect.Width - Padding * 2) / TotalTime.TotalMilliseconds;
        result *= milliseconds;
        return (int)(Rect.X + Padding + result);
    }
    
    private int getXPositionFromMilli(double milliseconds)
    {
        return getXPositionFromMilli((int)milliseconds);
    }
    
    private int getXPositionFromTimeSpan(TimeSpan timeSpan)
    {
        return getXPositionFromMilli(timeSpan.TotalMilliseconds);
    }

    private int InnerWidth
        => Rect.Width - Padding * 2;
}