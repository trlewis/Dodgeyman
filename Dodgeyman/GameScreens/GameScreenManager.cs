namespace Dodgeyman.GameScreens
{
    using System.Collections.Generic;
    using Code;
    using Models.Stats;
    using SFML.Graphics;
    using SFML.System;
    using SFML.Window;

    /// <summary>
    /// A stack-based manager for GameScreen objects. Handles activating, deactivating, and disposal
    /// of GameScreens it keeps track of. The top screen is the only one considered active. Does not
    /// keep track of framerate or sleep the program.
    /// 
    /// Also manages the RenderWindow object that the program uses.
    /// </summary>
    internal static class GameScreenManager
    {
        private static bool _isInitialized;
        private static readonly Stack<GameScreen> Screens = new Stack<GameScreen>();
        private static readonly Clock Clock = new Clock();
        private static RenderWindow _window;

        // ---------------------------------------------
        // PROPERTIES
        // ---------------------------------------------

        /// <summary>
        /// Whether or not the program is running. Is set to true when Initialize() is called
        /// and is set to false when Shutdown() is called.
        /// </summary>
        public static bool IsRunning { get; private set; }

        /// <summary>
        /// The Renderwindow that the program uses for displaying graphics. All input events
        /// should be registered to this
        /// </summary>
        public static RenderWindow RenderWindow
        {
            get { return _window; }
        }

        private static bool HasScreens
        {
            get { return Screens.Count > 0; }
        }

        // ---------------------------------------------
        // METHODS
        // ---------------------------------------------

        /// <summary>
        /// Calls the Draw() method on the top GameScreen in the stack. Passes in the 
        /// RenderWindow property of this class.
        /// </summary>
        public static void Draw()
        {
            if (!HasScreens)
                return;
            RenderWindow.Clear(Color.Black);
            Screens.Peek().Draw(RenderWindow);
            RenderWindow.Display();
        }

        /// <summary>
        /// Tells the Window to dispatch events
        /// </summary>
        public static void HandleEvents()
        {
            if(RenderWindow != null)
                RenderWindow.DispatchEvents();
        }

        /// <summary>
        /// Tells the GameManager to setup everything it needs to function. Should only be called once per program run.
        /// </summary>
        public static void Initialize()
        {
            if (_isInitialized)
                return;
            _isInitialized = true;

            GameStats.Initialize();
            IsRunning = true;

            var contextSettings = new ContextSettings { DepthBits = 24 };
            var vidMode = new VideoMode(ConfigHelper.ScreenSize, ConfigHelper.ScreenSize);
            _window = new RenderWindow(vidMode, "Dodgeyman", Styles.Close | Styles.Titlebar, contextSettings);
            _window.SetActive();
            _window.SetKeyRepeatEnabled(false);
            _window.Closed += (sender, args) => ShutDown();
        }

        /// <summary>
        /// Removes the top GameScreen on the stack and tells it to get ready to be deleted. If the last
        /// screen was removed ShutDown() is called.
        /// </summary>
        public static void PopScreen()
        {
            if (!HasScreens)
                return;

            var topScreen = Screens.Pop();
            topScreen.IsActive = false;
            topScreen.Dispose();

            if (HasScreens)
                Screens.Peek().IsActive = true;
            else
                ShutDown();
        }

        /// <summary>
        /// Deactivates the current top screen on the stack, initializes and activates the given GameScreen
        /// and adds it to the stack.
        /// </summary>
        /// <param name="screen">The new GameScreen to initialize, activate, and add to the stack.</param>
        public static void PushScreen(GameScreen screen)
        {
            if (HasScreens)
                Screens.Peek().IsActive = false;

            screen.Initialize(RenderWindow.Size);
            screen.IsActive = true;
            Screens.Push(screen);
        }

        /// <summary>
        /// Shuts down the GameScreenManager, and thus the Program
        /// </summary>
        public static void ShutDown()
        {
            while (HasScreens)
            {
                var screen = Screens.Pop();
                screen.IsActive = false;
                screen.Dispose();
            }

            _window.SetActive(false);
            _window.Close();
            GameStats.WriteStats();
            IsRunning = false;
        }

        /// <summary>
        /// Calls Update() on the top screen in the stack.
        /// </summary>
        public static void Update()
        {
            if (!HasScreens)
                return;
            Screens.Peek().Update(Clock.ElapsedTime);
        }
    }
}
