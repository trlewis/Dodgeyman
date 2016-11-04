namespace Dodgeyman
{
    using GameScreens.ArenaScreen;
    using SFML.Graphics;
    using SFML.Window;

    class Program
    {
        private static RenderWindow _window;
        private static ArenaScreen _gameScreen;

        static void Main()
        {
            CreateWindow();
            _gameScreen = new ArenaScreen(_window);

            while (_window.IsOpen)
            {
                //TODO: use sfml clock to decide how long to sleep
                System.Threading.Thread.Sleep(1000/60);
                _window.DispatchEvents();
                _window.Clear(Color.Black);
                _gameScreen.Update();
                _gameScreen.Draw();
                _window.Display();
            }
        }

        private static void CreateWindow()
        {
            var contextSettings = new ContextSettings { DepthBits = 24 };
            _window = new RenderWindow(new VideoMode(600, 600), "Dodgeyman", Styles.Close|Styles.Titlebar, contextSettings);
            _window.Closed += (sender, args) => _window.Close();
            _window.KeyPressed += (sender, args) =>
            {
                if (args.Code == Keyboard.Key.Escape)
                    _window.Close();
            };
            _window.SetActive();
        }

    }
}
