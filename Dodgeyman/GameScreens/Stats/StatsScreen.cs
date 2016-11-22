namespace Dodgeyman.GameScreens.Stats
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
        private StatLineGraph _pixelsMovedGraph;
        private float _statY;
        private StatLineGraph _scoreGraph;

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
            if(this._scoreGraph != null)
                this._scoreGraph.Dispose();
            if(this._pixelsMovedGraph != null)
                this._pixelsMovedGraph.Dispose();
        }

        public override void Draw(RenderTarget target)
        {
            this.DrawHeader(target);

            this._bf.StringSprite.Scale = StatScale;
            this._statY = 100;
            var gs = GameStats.Instance;
            this.DrawStat(target, String.Format("Games Played: {0}", gs.GamesPlayed));
            this._statY += StatYIncrement;

            //would be null if only 1 game has been played
            if (this._scoreGraph != null)
            {
                this.DrawStat(target, "Score");
                this._scoreGraph.GraphSprite.Position = new Vector2f(StatX, this._statY);
                this._statY += this._scoreGraph.GraphSprite.TextureRect.Height + 9;
                target.Draw(this._scoreGraph.GraphSprite);
            }

            this.DrawStat(target, String.Format("High: {0}", gs.HighScore));
            this.DrawStat(target, String.Format("Average: {0:0.00}", gs.AverageScore));
            this.DrawStat(target, String.Format("Total: {0}", gs.TotalScore));
            this._statY += StatYIncrement;

            this.DrawStat(target, String.Format("Total Color Switches: {0}", gs.TotalColorSwitches));
            this._statY += StatYIncrement;

            //would be null if only 1 game has been played
            if (this._pixelsMovedGraph != null)
            {
                this.DrawStat(target, "Pixels Moved");
                this._pixelsMovedGraph.GraphSprite.Position = new Vector2f(StatX, this._statY);
                this._statY += this._scoreGraph.GraphSprite.TextureRect.Height + 9;
                target.Draw(this._pixelsMovedGraph.GraphSprite);
            }

            this.DrawStat(target, String.Format("Average: {0:0.00}", gs.AveragePixelsMoved));
            this.DrawStat(target, String.Format("Total: {0}", gs.TotalPixelsMoved));
        }

        public override void Initialize()
        {
            this._bf = new BitmapFont("Assets/monochromeSimple.png");
            GameStats.Initialize();
            
            //if only 1 game has been played there really isn't a reason to make a graph...
            if (GameStats.Instance.GamesPlayed <= 1) 
                return;

            var screenSize = GameScreenManager.RenderWindow.Size;
            var graphSize = new Vector2u((uint) (screenSize.X - 2*StatX), 50);
            this._scoreGraph = new StatLineGraph(graphSize, GameStats.Instance.GetScoreHistory());
            this._pixelsMovedGraph = new StatLineGraph(graphSize, GameStats.Instance.GetPixelMovedHistory());
        }

        public override void Update(Time time) { }

        #endregion Inherited members

        // ---------------------------------------------
        // METHODS
        // ---------------------------------------------

        private void DrawHeader(RenderTarget target)
        {
            this._bf.RenderText("Stats");
            this._bf.StringSprite.Scale = TitleScale;
            var x = (GameScreenManager.RenderWindow.Size.X - (this._bf.StringSprite.TextureRect.Width*TitleScale.X))/2;
            this._bf.StringSprite.Position = new Vector2f(x, 20);
            target.Draw(this._bf.StringSprite);
        }

        private void DrawStat(RenderTarget target, string statString)
        {
            this._bf.RenderText(statString);
            this._bf.StringSprite.Position = new Vector2f(StatX, this._statY);
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
