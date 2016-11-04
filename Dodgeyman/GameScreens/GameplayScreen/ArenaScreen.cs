namespace Dodgeyman.GameScreens.GameplayScreen
{
    using SFML.Graphics;
    using SFML.System;

    internal class ArenaScreen
    {
        private const float PlayableAreaPercent = 0.8f;
        private RenderWindow RenderWindow { get; set; }
        private Player Player { get; set; }
        private RectangleShape ArenaBounds { get; set; }

        public ArenaScreen(RenderWindow target)
        {
            this.RenderWindow = target;
            this.Player = new Player(new Vector2f(target.Size.X/2.0f, target.Size.Y/2.0f));
            this.RenderWindow.KeyPressed += this.Player.KeyPressed;
            this.RenderWindow.KeyReleased += this.Player.KeyReleased;
            this.CreateArena(this.RenderWindow.Size);
        }

        private void CreateArena(Vector2u screenSize)
        {
            Vector2f arenaSize = new Vector2f(screenSize.X*PlayableAreaPercent, screenSize.Y*PlayableAreaPercent);
            //float width = screenSize.X*PlayableAreaPercent;
            //float height = screenSize.Y*PlayableAreaPercent;
            float dx = screenSize.X - arenaSize.X;
            float dy = screenSize.Y - arenaSize.Y;
            this.ArenaBounds = new RectangleShape(arenaSize);
            this.ArenaBounds.FillColor = Color.Transparent;
            this.ArenaBounds.Position = new Vector2f(dx/2, dy/2);
            this.ArenaBounds.OutlineColor = new Color(0x44,0x44,0x44);
            this.ArenaBounds.OutlineThickness = 3f;
        }

        public void Draw()
        {
            this.RenderWindow.Draw(this.ArenaBounds);
            this.RenderWindow.Draw(this.Player.PlayerSprite);
        }

        public void Update()
        {
            this.Player.Update();
        }
    }
}
