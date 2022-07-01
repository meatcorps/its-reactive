using Microsoft.Xna.Framework;

namespace ItsReactive.UI.Data.TimelineUIComponent;

public class TimelineItem
{
    public string Title;
    public object? ReferenceObject;
    public TimeSpan Position;
    public Color Color;

    public TimelineItem(string title, TimeSpan position, Color color)
    {
        Title = title;
        Position = position;
        Color = color;
    }
    
    public TimelineItem(string title, object? referenceObject, TimeSpan position, Color color)
    {
        Title = title;
        ReferenceObject = referenceObject;
        Position = position;
        Color = color;
    }
}