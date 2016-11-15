namespace Dodgeyman.GameScreens.ArenaScreen
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using SFML.Graphics;
    using SFML.System;
    using SFML.Window;

    internal class ArenaScreen : GameScreen
    {
        private const float PlayableAreaPercent = 0.7f;
        private const float ScoreScale = 10;

        private readonly List<DodgeLine> _lines = new List<DodgeLine>();
        private readonly Clock _arenaClock = new Clock();

        //this is so we don't count an already crossed line every frame
        private readonly List<DodgeLine> _crossedLines = new List<DodgeLine>();
        
        private int _lineSpawnTime = 2000; //in milliseconds, goes down as the game goes on
        private int _lastLineSpawnTime; //in milliseconds
        private bool _playerHit;
        private int _score;
        private BitmapFont.BitmapFont _bf;

        #region Inherited members

        //ActiveEntity
        protected override void Activate()
        {
            this.Player.IsActive = true;
            foreach (var line in this._lines)
                line.IsActive = true;
            GameScreenManager.Instance.RenderWindow.KeyPressed += this.KeyPressed;
        }

        //ActiveEntity
        protected override void Deactivate()
        {
            this.Player.IsActive = false;
            foreach (var line in this._lines)
                line.IsActive = false;
            GameScreenManager.Instance.RenderWindow.KeyPressed -= this.KeyPressed;
        }

        //GameScreen
        public override void Draw(RenderTarget target)
        {
            target.Draw(this.ArenaRectangle);
            target.Draw(this.Player.PlayerSprite);
            this.DrawScore(target);

            foreach (var line in this._lines)
                line.Draw(target);
        }

        //GameScreen
        public override void Initialize()
        {
            this.CreateArena();
            this.CreatePlayer();

            this._bf = new BitmapFont.BitmapFont("Assets/monochromeSimple.png");
            this._bf.StringSprite.Scale = new Vector2f(ScoreScale, ScoreScale);
            this._bf.StringSprite.Color = new Color(0xFF, 0xFF, 0xFF, 0x33);

            var clockTime = this._arenaClock.ElapsedTime;
            this._lastLineSpawnTime = clockTime.AsMilliseconds();
        }

        //GameScreen
        public override void Update(Time time)
        {
            this.Player.Update();
            this.HandleLineSpawning();
            this.UpdateLines();
        }

        #endregion Inherited members

        private RectangleShape ArenaRectangle { get; set; }
        private Player Player { get; set; }

        private void CreateArena()
        {
            var screenSize = GameScreenManager.Instance.RenderWindow.Size;
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

        private void HandleLineSpawning()
        {
            var clockTime = this._arenaClock.ElapsedTime.AsMilliseconds();
            if (clockTime - this._lastLineSpawnTime < this._lineSpawnTime)
                return;

            var windowSize = GameScreenManager.Instance.RenderWindow.Size;
            var dl = new DodgeLine(windowSize, this.Player);
            this._lines.Add(dl);
            this._lastLineSpawnTime = clockTime;
        }

        private void KeyPressed(object src, KeyEventArgs args)
        {
            if(args.Code == Keyboard.Key.Escape)
                GameScreenManager.Instance.PopScreen();
        }

        private void UpdateLines()
        {
            var finishedLines = new List<DodgeLine>();
            foreach (DodgeLine line in this._lines)
            {
                line.Update();
                if (line.IsCrossed && !this._crossedLines.Contains(line))
                {
                    this._crossedLines.Add(line);
                    if (line.IsCollided)
                        this._playerHit = true;
                    else
                        this._score++;
                }

                if(line.IsFinished)
                    finishedLines.Add(line);
            }

            foreach (DodgeLine line in finishedLines)
            {
                this._lines.Remove(line);
                this._crossedLines.Remove(line);
            }
        }
    }
}
