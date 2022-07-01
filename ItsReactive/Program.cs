using System;
using Autofac;
using Autofac.Extras.NLog;
using ItsReactive.Core.Adapters;
using ItsReactive.Core.Background;
using ItsReactive.Core.Helpers;
using ItsReactive.Core.Interfaces;
using ItsReactive.Core.Lists;
using ItsReactive.Core.Services;
using ItsReactive.Level;

namespace ItsReactive
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<NLogModule>();
            builder.RegisterType<PersistentLoaderAndSaver>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<MemoryBroker>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<HighScore>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<InputEvent>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<MainGame>().AsSelf().SingleInstance();
            builder.RegisterType<RenderHelper>().AsSelf().SingleInstance();
            builder.RegisterType<LevelExperience>().AsSelf().SingleInstance();
            builder.RegisterType<Dialog>().AsImplementedInterfaces().SingleInstance();
            // builder.RegisterType<Music>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<Effect>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterGeneric(typeof(LoggerAdapter<>)).AsImplementedInterfaces();
            
            builder.RegisterType<MainMenu>().AsSelf().SingleInstance();
            builder.RegisterType<SimpleLevel>().AsSelf().SingleInstance();

            builder.Register<ISceneManager>(provider =>
            {
                var sceneManager = new SceneManager(provider.Resolve<IBroker<Topic>>());

                sceneManager.Add(typeof(MainMenu), provider.Resolve<MainMenu>());
                sceneManager.Add(typeof(SimpleLevel), provider.Resolve<SimpleLevel>());

                return sceneManager;
            }).SingleInstance();
            
            builder.Register<IKeyValueDatabaseCollection<StorageType, GameKeyStorage>>(provider =>
            {
                var dataCollection = new DataCollection();
                dataCollection.SetKeyValueDatabase(StorageType.InMemory, new MemoryDatabase());
                dataCollection.SetKeyValueDatabase(StorageType.OnlyGame, new MemoryDatabase());
                var loader = provider.Resolve<IKeyValueLoader<GameKeyStorage>>();
                var saver = provider.Resolve<IKeyValueSaver<GameKeyStorage>>();
                if (saver != null && loader != null)
                    dataCollection.SetKeyValueDatabase(StorageType.Persistent, new PersistentDatabase(
                        loader, saver));
                return dataCollection;
            }).SingleInstance();

            var container = builder.Build();

            var game = container.Resolve<MainGame>();
            var sceneManager = container.Resolve<ISceneManager>();
            sceneManager.SwitchTo(typeof(SimpleLevel));
            
            using (game)
                game.Run();
            
            container.Resolve<IKeyValueSaver<GameKeyStorage>>().Save();
            
            container.Dispose();
        }
    }
}