namespace Dodgeyman.GameScreens
{
    using System;
    using Font;
    using Models.Stats;
    using SFML.Graphics;
    using SFML.System;
    using SFML.Window;

    class StatsScreen : GameScreen
    {
        private const float StatX = 20;
        private const float StatYIncrement = 20;
        private static readonly Vector2f StatScale = new Vector2f(1, 1);
        private static readonly Vector2f TitleScale = new Vector2f(3, 3);

        private BitmapFont _bf;
        private float _statY;

        // ---------------------------------------------
        // INHERITED MEMBERS
        // ---------------------------------------------

        #region Inherited members

        protected override void Activate()
        {
            GameScreenManager.RenderWindow.KeyPressed += this.HandleKeyPress;
        }

        protected override void Deactivate()
        {
            GameScreenManager.RenderWindow.KeyPressed -= this.HandleKeyPress;
        }

        public override void Dispose()
        {
            this._bf.Dispose();
        }

        public override void Draw(RenderTarget target)
        {
            this.DrawHeader(target);

            this._bf.StringSprite.Scale = StatScale;
            this._statY = 100;
            var gs = GameStats.Instance;
            this.DrawStat(target, String.Format("Games Played: {0}", gs.GamesPlayed));
            this.DrawStat(target, String.Format("High Score: {0}", gs.HighScore));
            this.DrawStat(target, String.Format("Average Score: {0:0.00}", gs.AverageScore));
            this.DrawStat(target, String.Format("Total Score: {0}", gs.TotalScore));
            this._statY += StatYIncrement;
            this.DrawStat(target, String.Format("Total Color Switches: {0}", gs.TotalColorSwitches));
            this._statY += StatYIncrement;
            this.DrawStat(target, String.Format("Average Pixels Moved: {0:0.00}", gs.AveragePixelsMoved));
            this.DrawStat(target, String.Format("Total Pixels Moved: {0}", gs.TotalPixelsMoved));
        }

        public override void Initialize()
        {
            this._bf = new BitmapFont("Assets/monochromeSimple.png");
        }

        public override void Update(Time time) { }

        #endregion Inherited members

        private void DrawHeader(RenderTarget target)
        {
            this._bf.RenderText("Stats");
            this._bf.StringSprite.Scale = TitleScale;
            var y = 20;
            var x = (GameScreenManager.RenderWindow.Size.X - (this._bf.StringSprite.TextureRect.Width*TitleScale.X))/2;
            this._bf.StringSprite.Position = new Vector2f(x, y);
            target.Draw(this._bf.StringSprite);
        }

        private void DrawStat(RenderTarget target, string statString)
        {
            this._bf.RenderText(statString);
            this._bf.StringSprite.Position = new Vector2f(StatX, _statY);
            target.Draw(this._bf.StringSprite);
            this._statY += StatYIncrement;
        }

        private void HandleKeyPress(object sender, KeyEventArgs args)
        {
            if(args.Code == Keyboard.Key.Escape)
                GameScreenManager.PopScreen();
        }
    }
}
