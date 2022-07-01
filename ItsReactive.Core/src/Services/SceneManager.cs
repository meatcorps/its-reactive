using ItsReactive.Core.Interfaces;
using ItsReactive.Core.Lists;

namespace ItsReactive.Core.Services;

public class SceneManager: ISceneManager
{
    private readonly Dictionary<Type, BaseGame> _scenes = new();
    private readonly IBroker<Topic> _broker;
    private Type? _lastScene = null;

    public SceneManager(IBroker<Topic> broker)
    {
        _broker = broker;
    }

    public void Add(Type scene, BaseGame gameBase)
    {
        _scenes.Add(scene, gameBase);
        gameBase.SetSceneManager(this);
    }

    public void SwitchTo(Type scene)
    {
        if (!_scenes.ContainsKey(scene) || _lastScene == scene)
            return;

        _lastScene = scene;
        var sceneScreen = _scenes[scene] as IScreen;
        _broker.Push(Topic.SwitchScene, sceneScreen);
    }

    public string[] AllScenes => _scenes.Values.Select(scene => scene.Name).ToArray();

    public void SwitchTo(string scene)
    {
        foreach (var possibleScene in _scenes)
        {
            if (possibleScene.Value.Name == scene)
            {
                SwitchTo(possibleScene.Key);
                return;
            }
        }
    }

    public Type CurrentScene => _lastScene;
}