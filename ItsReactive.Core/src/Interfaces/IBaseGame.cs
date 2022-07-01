using Microsoft.Extensions.DependencyInjection;

namespace ItsReactive.Core.Interfaces;

public interface IBaseGame
{
    public string Name { get; }
    public ISceneManager SceneManager { get; }
}