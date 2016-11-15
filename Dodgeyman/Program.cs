namespace Dodgeyman
{
    using GameScreens;
    using GameScreens.ArenaScreen;
    using SFML.Graphics;
    using SFML.System;
    using SFML.Window;

    static class Program
    {
        //private static RenderWindow _window;
        //private static ArenaScreen _gameScreen;
        private static GameScreenManager _gsm;

        static void Main()
        {
            //CreateWindow();
            //_gameScreen = new ArenaScreen(_window);
            //Clock clock = new Clock();
            //MainMenuScreen menu = new MainMenuScreen(_window);

            //while (_window.IsOpen)
            //{
            //    //TODO: use sfml clock to decide how long to sleep
            //    System.Threading.Thread.Sleep(1000/60);
            //    _window.DispatchEvents();
            //    _window.Clear(Color.Black);
            //    //_gameScreen.Update();
            //    //_gameScreen.Draw();
            //    menu.Update(clock.ElapsedTime);
            //    menu.Draw(_window);
            //    _window.Display();
            //}
            var clock = new Clock();
            var lastupdate = clock.ElapsedTime;
            const int frameRate = 1000/60; //in milliseconds
            _gsm = GameScreenManager.Instance;
            _gsm.PushScreen(new MainMenuScreen());

            while (GameScreenManager.Instance.RenderWindow.IsOpen)
            {
                var startTime = clock.ElapsedTime;
                _gsm.HandleEvents();
                _gsm.Update();
                _gsm.Draw();
                var endTime = clock.ElapsedTime;

                var len = (endTime - startTime).AsMilliseconds();
                if(len < frameRate)
                    System.Threading.Thread.Sleep(frameRate - len);
            }
        }

        //private static void CreateWindow()
        //{
        //    var contextSettings = new ContextSettings { DepthBits = 24 };
        //    _window = new RenderWindow(new VideoMode(600, 600), "Dodgeyman", Styles.Close|Styles.Titlebar, contextSettings);
        //    _window.Closed += (sender, args) => _window.Close();
        //    _window.KeyPressed += (sender, args) =>
        //    {
        //        if (args.Code == Keyboard.Key.Escape)
        //            _window.Close();
        //    };
        //    _window.SetActive();
        //}

    }
}
