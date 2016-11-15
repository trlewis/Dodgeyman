namespace Dodgeyman.GameScreens
{
    using System;
    using System.Collections.Generic;
    using SFML.Graphics;
    using SFML.System;
    using SFML.Window;

    internal class GameScreenManager : IDisposable
    {
        private static GameScreenManager _managerInstance;
        private readonly Stack<GameScreen> _screens = new Stack<GameScreen>();
        private readonly Clock _clock = new Clock();

        private GameScreenManager()
        {
            //not sure if this class should be in charge of the window. figure out where to put it later
            var contextSettings = new ContextSettings { DepthBits = 24 };
            this.RenderWindow = new RenderWindow(new VideoMode(600, 600), "Dodgeyman", Styles.Close | Styles.Titlebar, contextSettings);
            this.RenderWindow.SetActive();
            this.RenderWindow.Closed += (sender, args) => this.Dispose();
        }

        public static GameScreenManager Instance
        {
            get { return _managerInstance ?? (_managerInstance = new GameScreenManager()); }
        }

        private bool HasScreens
        {
            get { return this._screens.Count > 0; }
        }

        public RenderWindow RenderWindow { get; private set; }

        public void Draw()
        {
            if (!this.HasScreens)
                return;
            this.RenderWindow.Clear(Color.Black);
            this._screens.Peek().Draw(this.RenderWindow);
            this.RenderWindow.Display();
        }

        public void HandleEvents()
        {
            if(this.HasScreens && this.RenderWindow != null)
                this.RenderWindow.DispatchEvents();
        }

        public void PopScreen()
        {
            if (!this.HasScreens)
                return;

            var topScreen = this._screens.Pop();
            topScreen.Dispose();

            //if that was the last screen, close the program, otherwise set the next screen as active
            if (!this.HasScreens)
                this.Dispose();
            else
                this._screens.Peek().IsActive = true;
        }

        public void PushScreen(GameScreen screen)
        {
            if (this.HasScreens)
                this._screens.Peek().IsActive = false;

            screen.Initialize();
            screen.IsActive = true;
            this._screens.Push(screen);
        }

        public void Update()
        {
            if (!this.HasScreens)
                return;

            this._screens.Peek().Update(this._clock.ElapsedTime);
        }

        /// <summary>
        /// Disposes all screens, deactivates the window, and closes the program.
        /// </summary>
        public void Dispose()
        {
            while (this.HasScreens)
            {
                var gs = this._screens.Pop();
                gs.Dispose();
            }
            this.RenderWindow.SetActive(false);
            this.RenderWindow.Close();
        }
    }
}
