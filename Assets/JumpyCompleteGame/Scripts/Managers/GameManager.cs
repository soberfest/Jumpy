namespace Jumpy
{
    using UnityEngine;

    /// <summary>
    /// The first script that is run in the game
    /// All initializations should be done here
    /// </summary>
    public class GameManager : SingleReference<GameManager>
    {
        /// <summary>
        /// A reference to all the important game properties should be put here
        /// </summary>
        private GameProgressManager gameStatus;
        public GameProgressManager GameStatus
        {
            get
            {
                if (gameStatus == null)
                {
                    gameStatus = new GameProgressManager();
                }
                return gameStatus;
            }
        }


        /// <summary>
        /// A reference of UI loader script to be used from entire game
        /// </summary>
        private AssetsLoaderManager assetsLoader;
        public AssetsLoaderManager AssetsLoader
        {
            get
            {
                if (assetsLoader == null)
                {
                    assetsLoader = gameObject.AddComponent<AssetsLoaderManager>();
                }
                return assetsLoader;
            }
        }


        /// <summary>
        /// A reference of sound loader script to be used from the entire game
        /// </summary>
        SoundLoaderManager soundLoader;
        public SoundLoaderManager SoundLoader
        {
            get
            {
                if (soundLoader == null)
                {
                    soundLoader = gameObject.AddComponent<SoundLoaderManager>();
                }
                return soundLoader;
            }
        }

        TweenManager tweenManager;
        public TweenManager Tween
        {
            get
            {
                if(tweenManager == null)
                {
                    tweenManager = gameObject.AddComponent<TweenManager>();
                }
                return tweenManager;
            }
        }


        /// <summary>
        /// All game initializations should be done here
        /// </summary>
        private void Start()
        {
            //Keep this object for the entire game session
            DontDestroyOnLoad(gameObject);

            //Keep the screen active all the time
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            //Initialize user input capabilities
            gameObject.AddComponent<UserInputManager>();
            
            //Load saved data
            GameStatus.LoadGameStatus();

            //Preload game UI
            AssetsLoader.PrepareGameUI();

            //Initialize sound with the previous saved values
            SoundLoader.InitializeSoundManager(gameObject, GameStatus.FXVolume, GameStatus.MusicVolume);

            //Start background music
            SoundLoader.AddMusic("Music");

            //Start the game
            LoadGraphics();
        }

        private void CompleteMethod(bool arg0, string arg1)
        {
            Debug.Log(arg0 + " " + arg1);
        }


        /// <summary>
        /// Loads the game graphic
        /// </summary>
        private void LoadGraphics()
        {
            AssetsLoader.LoadPopup(GamePopups.TitleScreenPopup, null);
            Debug.Log("Loaded something (graphics)");
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus == false)
            {
                //if user left your app schedule all your notifications
                //GleyNotifications.SendNotification("Jumpy", "It is time to beat your high score", new System.TimeSpan(24, 0, 0), null, null, "Opened from notification");
            }
            else
            {
                //call initialize when user returns to your app to cancel all pending notifications
               // GleyNotifications.Initialize();
            }
        }
    }
}
