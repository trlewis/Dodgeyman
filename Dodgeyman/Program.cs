namespace Dodgeyman
{
    using GameScreens;
    using SFML.System;

    static class Program
    {
        static void Main()
        {
            var clock = new Clock();
            const int frameRate = 1000/60;
            GameScreenManager.Initialize();
            GameScreenManager.PushScreen(new MainMenuScreen());

            while (GameScreenManager.IsRunning)
            {
                var startTime = clock.ElapsedTime;
                GameScreenManager.HandleEvents();
                GameScreenManager.Update();
                GameScreenManager.Draw();
                var endTime = clock.ElapsedTime;

                var len = (endTime - startTime).AsMilliseconds();
                if(len < frameRate)
                    System.Threading.Thread.Sleep(frameRate - len);
            }
        }
    }
}
