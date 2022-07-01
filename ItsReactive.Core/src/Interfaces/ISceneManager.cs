using System.ComponentModel.Design;

namespace ItsReactive.Core.Interfaces;

public interface ISceneManager
{
    void Add(Type scene, BaseGame gameBase);
    void SwitchTo(Type scene);
    void SwitchTo(string scene);
    Type CurrentScene { get; }
    string[] AllScenes { get; }
}