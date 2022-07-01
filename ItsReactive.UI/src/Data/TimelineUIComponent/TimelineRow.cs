namespace ItsReactive.UI.Data.TimelineUIComponent;

public class TimelineRow
{
    public string Title;
    public List<TimelineItem> Items { get; }

    public TimelineRow(string title)
    {
        Title = title;
        Items = new List<TimelineItem>();
    }
    
    public TimelineRow(string title, IEnumerable<TimelineItem> items)
    {
        Title = title;
        Items = new List<TimelineItem>();
        Items.AddRange(items.ToArray());
    }
}