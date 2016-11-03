using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dodgeyman
{
    using System.Runtime.Remoting.Contexts;
    using System.Security.AccessControl;
    using System.Threading;
    using GameScreens.GameplayScreen;
    using SFML.Graphics;
    using SFML.System;
    using SFML.Window;
    

    class Program
    {
        private static RenderWindow window;
        private static bool running = true;
        private static GameplayScreen gameScreen;

        static void Main(string[] args)
        {
            CreateWindow();
            gameScreen = new GameplayScreen(window);

            while (window.IsOpen)
            {
                //TODO: use sfml clock to decide how long to sleep
                System.Threading.Thread.Sleep(1000/60);
                window.DispatchEvents();
                window.Clear(Color.Black);
                gameScreen.Update();
                gameScreen.Draw();
                window.Display();
            }
        }

        private static void CreateWindow()
        {
            ContextSettings contextSettings = new ContextSettings();
            contextSettings.DepthBits = 24;

            window = new RenderWindow(new VideoMode(640, 480), "blah", Styles.Close|Styles.Titlebar);
            window.Closed += (sender, args) => window.Close();
            window.SetActive();
        }

    }
}
