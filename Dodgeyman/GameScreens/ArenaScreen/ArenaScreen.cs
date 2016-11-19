namespace Dodgeyman.GameScreens.ArenaScreen
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Lines;
    using SFML.Graphics;
    using SFML.System;
    using SFML.Window;

    internal class ArenaScreen : GameScreen
    {
        private const float PlayableAreaPercent = 0.7f;
        private const float ScoreScale = 15;

        private readonly List<DodgeLine> _lines = new List<DodgeLine>();
        private readonly Queue<DodgeLine> _finishedLines = new Queue<DodgeLine>(); //lines to remove from list
        private readonly Clock _arenaClock = new Clock();
        
        private int _lineSpawnTime = 1300; //in milliseconds, goes down as the game goes on
        private int _lastLineSpawnTime; //in milliseconds
        private bool _gameOver;
        private GameOverPanel _gameoverPanel;
        private int _score;
        private BitmapFont.BitmapFont _bf;

        // ---------------------------------------------
        // PROPERTIES
        // ---------------------------------------------

        private RectangleShape ArenaRectangle { get; set; }
        private Player Player { get; set; }

        // ---------------------------------------------
        // INHERITED MEMBERS
        // ---------------------------------------------

        #region Inherited members

        //IDisposable
        public override void Dispose()
        {
            this.IsActive = false;
            this._bf.Dispose();
            //TODO: dispose player and lines
        }

        //ActiveEntity
        protected override void Activate()
        {
            this.Player.IsActive = true;
            foreach (var line in this._lines)
                line.IsActive = true;
            GameScreenManager.RenderWindow.KeyPressed += this.KeyPressed;
        }

        //ActiveEntity
        protected override void Deactivate()
        {
            this.Player.IsActive = false;
            foreach (var line in this._lines)
                line.IsActive = false;
            GameScreenManager.RenderWindow.KeyPressed -= this.KeyPressed;
        }

        //GameScreen
        public override void Draw(RenderTarget target)
        {
            target.Draw(this.ArenaRectangle);
            this.DrawScore(target);

            foreach (var line in this._lines)
                line.Draw(target);

            target.Draw(this.Player.PlayerSprite);

            if (this._gameOver && this._gameoverPanel != null)
                this._gameoverPanel.Draw(target);
        }

        //GameScreen
        public override void Initialize()
        {
            this.CreateArena();
            this.CreatePlayer();

            this._bf = new BitmapFont.BitmapFont("Assets/5x5numbers.png");
            this._bf.StringSprite.Scale = new Vector2f(ScoreScale, ScoreScale);
            this._bf.StringSprite.Color = new Color(0xFF, 0xFF, 0xFF, 0x33);
            this.SetScoreTint();

            var clockTime = this._arenaClock.ElapsedTime;
            this._lastLineSpawnTime = clockTime.AsMilliseconds();
        }

        //GameScreen
        public override void Update(Time time)
        {
            if (!this._gameOver)
            {
                this.Player.Update();
                this.HandleLineSpawning();
                this.UpdateLines();
            }
            else if (this._gameOver && this._gameoverPanel != null)
                this._gameoverPanel.Update();
        }

        #endregion Inherited members

        // ---------------------------------------------
        // METHODS
        // ---------------------------------------------

        private void CreateArena()
        {
            var screenSize = GameScreenManager.RenderWindow.Size;
            var width = (float) Math.Floor(screenSize.X*PlayableAreaPercent);
            var height = (float) Math.Floor(screenSize.Y*PlayableAreaPercent);
            var arenaSize = new Vector2f(width, height);
            var dx = screenSize.X - arenaSize.X;
            var dy = screenSize.Y - arenaSize.Y;
            this.ArenaRectangle = new RectangleShape(arenaSize)
                                  {
                                      FillColor = Color.Transparent,
                                      Position = new Vector2f(dx/2, dy/2),
                                      OutlineColor = new Color(0x44, 0x44, 0x44),
                                      OutlineThickness = 3f
                                  };
        }

        private void CreatePlayer()
        {
            var arenaBounds = new FloatRect(this.ArenaRectangle.Position.X,
                this.ArenaRectangle.Position.Y,
                this.ArenaRectangle.Size.X,
                this.ArenaRectangle.Size.Y);
            this.Player = new Player(arenaBounds);
            this.Player.ColorChanged += (sender, args) => this.SetScoreTint();
        }

        private void DrawSpawnTime(RenderTarget target)
        {
            var text = this._lineSpawnTime.ToString(CultureInfo.InvariantCulture);
            if (this._gameOver)
                text = "0 " + text;
            this._bf.RenderText(text);
            this._bf.StringSprite.Position = new Vector2f(0, 0);
            target.Draw(this._bf.StringSprite);
        }

        private void DrawScore(RenderTarget target)
        {
            this._bf.RenderText(this._score.ToString(CultureInfo.InvariantCulture));
            var textWidth = this._bf.StringSprite.TextureRect.Width*this._bf.StringSprite.Scale.X;
            var textHeight = this._bf.StringSprite.TextureRect.Height*this._bf.StringSprite.Scale.Y;
            var x = (target.Size.X - textWidth)/2;
            var y = (target.Size.Y - textHeight)/2;
            this._bf.StringSprite.Position = new Vector2f(x, y);
            target.Draw(this._bf.StringSprite);
        }

        private void GameOver()
        {
            this._gameOver = true;
            this.SetScoreTint();
            foreach (var line in this._lines)
                line.IsActive = false;
            this.Player.IsActive = false;
            this._gameoverPanel = new GameOverPanel();
        }

        private void HandleLineSpawning()
        {
            var clockTime = this._arenaClock.ElapsedTime.AsMilliseconds();
            if (clockTime - this._lastLineSpawnTime < this._lineSpawnTime)
                return;

            var dl = DodgeLineGenerator.GenerateLine(this.Player);
            dl.Crossed += this.LineCrossed;
            dl.Finished += this.LineFinished;
            this._lines.Add(dl);
            this._lastLineSpawnTime = clockTime;
        }

        private void KeyPressed(object src, KeyEventArgs args)
        {
            if (args.Code == Keyboard.Key.Escape)
            {
                GameScreenManager.PopScreen();
            }
            if(args.Code == Keyboard.Key.Return && this._gameOver)
            {
                GameScreenManager.PopScreen();
                GameScreenManager.PushScreen(new ArenaScreen());
            }
        }

        /// <summary>
        /// Listener for the Crossed event on DodgeLine.
        /// </summary>
        /// <param name="sender">The object that's finished. Probably a DodgeLine or object with similar purpose.</param>
        /// <param name="args"></param>
        private void LineCrossed(object sender, LineCrossedEventArgs args)
        {
            if (!args.Hit && !this._gameOver)
            {
                this._score++;
                var timeReduction = 100f / Math.Sqrt(this._score);
                //this._lineSpawnTime = (int)Math.Max(this._lineSpawnTime - timeReduction, 400);
                var newSpawnTime = (int)Math.Max(this._lineSpawnTime - timeReduction, 400);
                //let it bring it down by 1ms after it hits "minimum"
                this._lineSpawnTime = (this._lineSpawnTime == newSpawnTime) ? this._lineSpawnTime - 1 : newSpawnTime;
            }
            else
            {
                this.GameOver();
            }
        }

        /// <summary>
        /// Listener for the Finished event on DodgeLine. What to do when a line is "finished". 
        /// That is, when it is no longer useful. Called by lines as an event.
        /// </summary>
        /// <param name="sender">The object that's finished. Probably a DodgeLine or some an object with similar purpose.</param>
        /// <param name="args">Empty, calling this method is just a signal.</param>
        private void LineFinished(object sender, EventArgs args)
        {
            if(sender is DodgeLine)
                this._finishedLines.Enqueue(sender as DodgeLine);
        }

        private void SetScoreTint()
        {
            //var pc = this.Player.Color;
            var pc = this._gameOver ? Color.White : this.Player.Color;
            //red tints a little darker than cyan, so adjust for that
            var alpha = (byte)(pc.Equals(Color.Red) ? 0x44 : 0x33);
            this._bf.StringSprite.Color = new Color(pc.R, pc.G, pc.B, alpha);
        }

        private void UpdateLines()
        {
            foreach (DodgeLine line in this._lines)
                line.Update();

            while (this._finishedLines.Count > 0)
                this._lines.Remove(this._finishedLines.Dequeue());
        }
    }
}
