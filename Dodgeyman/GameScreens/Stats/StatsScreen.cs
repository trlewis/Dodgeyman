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
        private static readonly Vector2f StatScale = new Vector2f(2, 2);
        private static readonly Vector2f TitleScale = new Vector2f(6, 6);
        private const string ScoreLineTemplate = "HIGH: {0}   AVERAGE: {1:0.00}  TOTAL: {2}";
        private const string PixelsLineTemplate = "AVERAGE: {0:0.00}  TOTAL: {1}";

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

            this._statY = this._bf.StringSprite.Position.Y + this._bf.ScreenSize.Y + 10;
            this._bf.StringSprite.Scale = StatScale;
            var gs = GameStats.Instance;
            this.DrawStat(target, String.Format("GAMES PLAYED: {0}", gs.GamesPlayed));
            this.DrawStat(target, String.Format("TOTAL COLOR SWITCHES: {0}", gs.TotalColorSwitches));
            this._statY += this._bf.ScreenLineHeight;

            //would be null if only 1 game has been played
            if (this._scoreGraph != null)
            {
                this.DrawStat(target, "SCORE");
                this._scoreGraph.GraphSprite.Position = new Vector2f(StatX, this._statY);
                this._statY += this._scoreGraph.GraphSprite.TextureRect.Height + 9;
                target.Draw(this._scoreGraph.GraphSprite);
            }

            var scoreLine = string.Format(ScoreLineTemplate, gs.HighScore, gs.AverageScore, gs.TotalScore);
            this.DrawStat(target, scoreLine);
            this._statY += this._bf.ScreenLineHeight;

            //would be null if only 1 game has been played
            if (this._pixelsMovedGraph != null)
            {
                this.DrawStat(target, "PIXELS MOVED");
                this._pixelsMovedGraph.GraphSprite.Position = new Vector2f(StatX, this._statY);
                this._statY += this._scoreGraph.GraphSprite.TextureRect.Height + 9;
                target.Draw(this._pixelsMovedGraph.GraphSprite);
            }

            var pxLine = string.Format(PixelsLineTemplate, gs.AveragePixelsMoved, gs.TotalPixelsMoved);
            this.DrawStat(target, pxLine);
        }

        public override void Initialize()
        {
            this._bf = new BitmapFont("Assets/5x5all.png");
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
            this._bf.RenderText("STATS");
            this._bf.StringSprite.Scale = TitleScale;
            var x = (target.Size.X - this._bf.ScreenSize.X)/2;
            this._bf.StringSprite.Position = new Vector2f(x, 20);
            target.Draw(this._bf.StringSprite);
        }

        private void DrawStat(RenderTarget target, string statString)
        {
            this._bf.RenderText(statString);
            this._bf.StringSprite.Position = new Vector2f(StatX, this._statY);
            target.Draw(this._bf.StringSprite);
            this._statY += this._bf.ScreenLineHeight;
        }

        private void HandleKeyPress(object sender, KeyEventArgs args)
        {
            if(args.Code == Keyboard.Key.Escape)
                GameScreenManager.PopScreen();
        }
    }
}
