using ItsReactive.Core.Interfaces;

namespace ItsReactive.Core.Interfaces;

public interface IBackgroundWorker: IScreen
{
    public bool AlwaysOn { get; }
    public bool UpdateAndDrawLast { get; }
}