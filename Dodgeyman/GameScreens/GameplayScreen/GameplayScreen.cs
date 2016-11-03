namespace Dodgeyman.GameScreens.GameplayScreen
{
    using SFML.Graphics;
    using SFML.System;

    internal class GameplayScreen
    {
        private RenderWindow RenderWindow { get; set; }
        private Player Player { get; set; }

        public GameplayScreen(RenderWindow target)
        {
            this.RenderWindow = target;
            this.Player = new Player(new Vector2f(target.Size.X/2.0f, target.Size.Y/2.0f));
            this.RenderWindow.KeyPressed += this.Player.KeyPressed;
            this.RenderWindow.KeyReleased += this.Player.KeyReleased;
        }


        public void Draw()
        {
            this.RenderWindow.Draw(this.Player.PlayerSprite);
        }

        public void Update()
        {
            this.Player.UpdatePosition();
        }
    }
}
