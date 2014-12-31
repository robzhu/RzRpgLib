using RzAspects;
using System;
using System.Diagnostics;

namespace RzRpgLib
{
    public static class UpdateGroupIds 
    {
        public const int Always = 0;        //Updates all the time
        public const int GameLogic = 1;         //Updates the game logic, such as resource regeneration
        public const int Animation = 2;      //Updates animations
    }

    public interface IGame
    {
        IUpdateService DefaultClock { get; }
        IUpdateService UpdateClock { get; }
        IUpdateService ActionClock { get; }
        IUpdateService AnimationClock { get; }

        void Pause();
        void Unpause();
        void CollectGarbage();
    }

    public class Game : IGame
    {
        public static Game Instance { get; private set; }
        public static bool Initialized { get; private set; }
        public static IGame Init( Func<IUpdateEventSource> frameRenderEventProvider )
        {
            if( Initialized ) return Instance;
            
            Instance = new Game( frameRenderEventProvider );

            IoCContainer.RegisterSingle<IGame>( () => 
            { 
                return Instance; 
            } );

            IoCContainer.RegisterSingle<IBucketUpdateService>( () =>
            {
                return Instance.Clock;
            } );

            Initialized = true;

            return Instance;
        }

        private IBucketUpdateService Clock { get; set; }

        public IUpdateService DefaultClock { get; private set; }
        public IUpdateService UpdateClock { get; private set; }
        public IUpdateService ActionClock { get; private set; }
        public IUpdateService AnimationClock { get; private set; }

        public Game( Func<IUpdateEventSource> frameRenderEventProvider )
        {
            Debug.Assert( frameRenderEventProvider != null );
            Clock = new BucketUpdateService( frameRenderEventProvider() );

            DefaultClock = Clock.GetUpdateServiceById( UpdateGroupIds.Always );
            DefaultClock.Name = "Default Clock (Always Updates)";

            UpdateClock = Clock.GetUpdateServiceById( UpdateGroupIds.GameLogic );
            UpdateClock.Name = "Game Logic Clock (Updates when main game loop is running)";

            AnimationClock = Clock.GetUpdateServiceById( UpdateGroupIds.Animation );
            AnimationClock.Name = "Animation Clock (Updates animations independent of game loop)";
        }

        public void CollectGarbage()
        {
            GC.Collect();
            Clock.CollectGarbage();
        }

        public void Pause()
        {
            Clock.PauseAll();
        }

        public void Unpause()
        {
            Clock.UnpauseAll();
        }
    }

    public static class GameInstance
    {
        private static IGame _instance;
        private static IGame Instance
        {
            get
            {
                if( _instance == null )
                {
                    try
                    {
                        _instance = IoCContainer.GetInstance<IGame>();
                    }
                    catch { }
                    //if( _instance == null ) throw new ArgumentNullException();
                }
                return _instance;
            }
        }

        public static IUpdateService DefaultClock { get { return Instance.DefaultClock; } }
        public static IUpdateService UpdateClock { get { return Instance.UpdateClock; } }
        public static IUpdateService ActionClock { get { return Instance.ActionClock; } }
        public static IUpdateService AnimationClock { get { return Instance.AnimationClock; } }

        public static void Pause()
        {
            Instance.Pause();
        }

        public static void Unpause()
        {
            Instance.Unpause();
        }

        public static void CollectGarbage()
        {
            Instance.CollectGarbage();
        }
    }
}
