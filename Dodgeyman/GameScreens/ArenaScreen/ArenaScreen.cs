namespace Dodgeyman.GameScreens.ArenaScreen
{
    using System;
    using System.Collections.Generic;
    using SFML.Graphics;
    using SFML.System;

    internal class ArenaScreen
    {
        private const float PlayableAreaPercent = 0.7f;

        private readonly List<DodgeLine> _lines = new List<DodgeLine>();
        private readonly Clock _arenaClock = new Clock();
        private readonly List<DodgeLine> _crossedLines = new List<DodgeLine>();
        
        private int _lineSpawnTime = 2000; //in milliseconds, goes down as the game goes on
        private int _lastLineSpawnTime; //in milliseconds
        private bool _playerHit;
        private int _score;

        public ArenaScreen(RenderWindow target)
        {
            this.RenderWindow = target;
            this.CreateArena(this.RenderWindow.Size);
            this.CreatePlayer();
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
            this.ArenaRectangle = new RectangleShape(arenaSize);
            this.ArenaRectangle.FillColor = Color.Transparent;
            this.ArenaRectangle.Position = new Vector2f(dx/2, dy/2);
            this.ArenaRectangle.OutlineColor = new Color(0x44,0x44,0x44);
            this.ArenaRectangle.OutlineThickness = 3f;
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
            foreach(var line in this._lines)
                line.Draw(this.RenderWindow);
        }

        public void Update()
        {
            this.Player.Update();
            this.HandleLineSpawning();
            this.UpdateLines();
            this.RenderWindow.SetTitle(String.Format("Dodgeyman - Score: {0}", this._score));
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
