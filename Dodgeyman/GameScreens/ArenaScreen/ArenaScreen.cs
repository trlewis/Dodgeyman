namespace Dodgeyman.GameScreens.ArenaScreen
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using SFML.Graphics;
    using SFML.System;

    internal class ArenaScreen
    {
        private const float PlayableAreaPercent = 0.7f;
        private const float ScoreScale = 10;

        private readonly List<DodgeLine> _lines = new List<DodgeLine>();
        private readonly Clock _arenaClock = new Clock();
        private readonly List<DodgeLine> _crossedLines = new List<DodgeLine>();
        
        private int _lineSpawnTime = 2000; //in milliseconds, goes down as the game goes on
        private int _lastLineSpawnTime; //in milliseconds
        private bool _playerHit;
        private int _score;
        private readonly BitmapFont.BitmapFont _bf;

        public ArenaScreen(RenderWindow target)
        {
            this.RenderWindow = target;
            this.CreateArena(this.RenderWindow.Size);
            this.CreatePlayer();

            this._bf = new BitmapFont.BitmapFont("Assets/monochromeSimple.png");
            this._bf.Sprite.Scale = new Vector2f(ScoreScale, ScoreScale);
            this._bf.Sprite.Color = new Color(0xFF, 0xFF, 0xFF, 0x33);

            var clockTime = this._arenaClock.ElapsedTime;
            this._lastLineSpawnTime = clockTime.AsMilliseconds();
        }

        private RectangleShape ArenaRectangle { get; set; }
        private RenderWindow RenderWindow { get; set; }
        private Player Player { get; set; }

        private void CreateArena(Vector2u screenSize)
        {
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
            var initPos = new Vector2f(this.RenderWindow.Size.X/2.0f, this.RenderWindow.Size.Y/2.0f);
            var arenaBounds = new FloatRect(this.ArenaRectangle.Position.X,
                this.ArenaRectangle.Position.Y,
                this.ArenaRectangle.Size.X,
                this.ArenaRectangle.Size.Y);
            this.Player = new Player(initPos, arenaBounds);
            this.RenderWindow.KeyPressed += this.Player.KeyPressed;
            this.RenderWindow.KeyReleased += this.Player.KeyReleased;
        }

        public void Draw()
        {
            this.RenderWindow.Draw(this.ArenaRectangle);
            this.RenderWindow.Draw(this.Player.PlayerSprite);
            this.DrawScore();

            foreach (var line in this._lines)
                line.Draw(this.RenderWindow);
        }

        private void DrawScore()
        {
            this._bf.RenderText(this._score.ToString(CultureInfo.InvariantCulture));
            var textWidth = this._bf.Sprite.TextureRect.Width*this._bf.Sprite.Scale.X;
            var textHeight = this._bf.Sprite.TextureRect.Height*this._bf.Sprite.Scale.Y;
            var x = (this.RenderWindow.Size.X - textWidth)/2;
            var y = (this.RenderWindow.Size.Y - textHeight)/2;
            this._bf.Sprite.Position = new Vector2f(x, y);
            this.RenderWindow.Draw(this._bf.Sprite);
        }


        public void Update()
        {
            this.Player.Update();
            this.HandleLineSpawning();
            this.UpdateLines();
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

        private void HandleLineSpawning()
        {
            var clockTime = this._arenaClock.ElapsedTime.AsMilliseconds();
            if (clockTime - this._lastLineSpawnTime < this._lineSpawnTime)
                return;

            var dl = new DodgeLine(this.RenderWindow.Size, this.Player);
            this._lines.Add(dl);
            this._lastLineSpawnTime = clockTime;
        }
    }
}
