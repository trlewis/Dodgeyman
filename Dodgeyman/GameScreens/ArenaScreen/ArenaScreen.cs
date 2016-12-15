namespace Dodgeyman.GameScreens.ArenaScreen
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Code;
    using Font;
    using Lines;
    using Lines.LinePatterns;
    using Models.Stats;
    using SFML.Graphics;
    using SFML.System;
    using SFML.Window;

    internal class ArenaScreen : GameScreen
    {
        private const float PlayableAreaPercent = 0.7f;
        private const int ParticlesPerCross = 40;
        private const float ScoreScale = 15;

        private readonly Clock _arenaClock = new Clock();
        private readonly Queue<DodgeLine> _finishedLines = new Queue<DodgeLine>(); //lines to remove from list
        private readonly Queue<Particle> _finishedParticles = new Queue<Particle>(); //particles to remove from the list
        private readonly List<DodgeLine> _lines = new List<DodgeLine>();
        private readonly List<Particle> _particles = new List<Particle>();
        private readonly SessionStats _stats = new SessionStats();
        
        private readonly Random _rand = new Random();
        
        private BitmapFont _bf;
        private bool _gameOver;
        private GameOverPanel _gameoverPanel;
        private int _lastLineSpawnTime; //in milliseconds
        private int _lineSpawnTime = 1500; //in milliseconds, goes down as the game goes on

        // ---------------------------------------------
        // PROPERTIES
        // ---------------------------------------------

        private RectangleShape ArenaRectangle { get; set; }

        private Player Player { get; set; }

        // ---------------------------------------------
        // INHERITED MEMBERS
        // ---------------------------------------------

        #region Inherited members

        // ActiveEntity
        protected override void Deactivate()
        {
            this.Player.IsActive = false;
            foreach (var line in this._lines)
                line.IsActive = false;
            GameScreenManager.RenderWindow.KeyPressed -= this.KeyPressed;
        }

        // GameScreen
        public override void Draw(RenderTarget target)
        {
            target.Draw(this.ArenaRectangle);
            this.DrawScore(target);

            foreach(var particle in this._particles)
                particle.Draw(target);

            foreach (var line in this._lines)
                line.Draw(target);

            target.Draw(this.Player.PlayerSprite);

            if (this._gameOver && this._gameoverPanel != null)
                this._gameoverPanel.Draw(target);
        }

        // GameScreen
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

            //always update particles
            this.UpdateParticles();
        }

        // IDisposable
        public override void Dispose()
        {
            this.IsActive = false;
            this._bf.Dispose();
            foreach (var line in this._lines)
                line.Dispose();
            this.Player.Dispose();
        }

        // ActiveEntity
        protected override void Activate()
        {
            this.Player.IsActive = true;
            foreach (var line in this._lines)
                line.IsActive = true;
            GameScreenManager.RenderWindow.KeyPressed += this.KeyPressed;
        }

        // GameScreen
        protected override void OnInitialize()
        {
            this.CreateArena();
            this.CreatePlayer();

            //hopefully there won't ever be more than 15 lines worth of particles on screen at a time.
            this._particles.Capacity = 15*ParticlesPerCross;

            this._bf = new BitmapFont(ConfigHelper.NumberFontLocation);
            this._bf.StringSprite.Scale = new Vector2f(ScoreScale, ScoreScale);
            this._bf.StringSprite.Color = new Color(0xFF, 0xFF, 0xFF, 0x33);
            this.SetScoreTint();

            var clockTime = this._arenaClock.ElapsedTime;
            this._lastLineSpawnTime = clockTime.AsMilliseconds();

            LinePatternGenerator.Initialize();
        }

        #endregion Inherited members

        // ---------------------------------------------
        // METHODS
        // ---------------------------------------------

        /// <summary>
        /// Takes care of all the event registering for the line and adds it to the collection
        /// </summary>
        /// <param name="line">The line to add to the active collection.</param>
        private void AddLine(DodgeLine line)
        {
            line.Crossed += this.LineCrossed;
            line.Finished += this.LineFinished;
            this._lines.Add(line);
        }

        private void CreateArena()
        {
            var width = (float) Math.Floor(this.TargetSize.X*PlayableAreaPercent);
            var height = (float) Math.Floor(this.TargetSize.Y*PlayableAreaPercent);
            var arenaSize = new Vector2f(width, height);
            var dx = this.TargetSize.X - arenaSize.X;
            var dy = this.TargetSize.Y - arenaSize.Y;
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
            this.Player.ColorChanged += (sender, args) =>
            {
                this.SetScoreTint();
                this._stats.ColorSwitches++;
            };
            this.Player.PlayerMoved += (sender, i) => this._stats.PixelsMoved += i;
        }

        private void DrawScore(RenderTarget target)
        {
            this._bf.RenderText(this._stats.Score.ToString(CultureInfo.InvariantCulture));
            var textWidth = this._bf.StringSprite.TextureRect.Width*this._bf.StringSprite.Scale.X;
            var textHeight = this._bf.StringSprite.TextureRect.Height*this._bf.StringSprite.Scale.Y;
            var x = (target.Size.X - textWidth)/2;
            var y = (target.Size.Y - textHeight)/2;
            this._bf.StringSprite.Position = new Vector2f(x, y);
            target.Draw(this._bf.StringSprite);
        }

        private void GameOver()
        {
            //TODO: spawn player death particles
            this._gameOver = true;
            this.SetScoreTint();
            foreach (var line in this._lines)
                line.IsActive = false;
            this.Player.IsActive = false;
            GameStats.Initialize();
            GameStats.Instance.AddSessionStats(this._stats);
            this._gameoverPanel = new GameOverPanel(this.TargetSize);
        }

        private void HandleLineSpawning()
        {
            var clockTime = this._arenaClock.ElapsedTime.AsMilliseconds();

            if (clockTime - this._lastLineSpawnTime < this._lineSpawnTime)
                return;

            var whatToSpawn = this._rand.Next(100);
            if (whatToSpawn < 30)
            {
                foreach (var lt in LinePatternGenerator.GetRandomPatternTemplates())
                    this.AddLine(lt.CreateLine(this.Player, this.TargetSize));
                //give patterns a little more time until next lines added
                this._lastLineSpawnTime = clockTime + (this._lineSpawnTime/2);
            }
            else
            {
                var dl = DodgeLineGenerator.GenerateLine(this.Player, this.TargetSize);
                this.AddLine(dl);
                this._lastLineSpawnTime = clockTime;
            }

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
                this._stats.Score++;
                var timeReduction = 100f / Math.Sqrt(this._stats.Score);
                const int minSpawnTime = 400;
                var newSpawnTime = (int)Math.Max(this._lineSpawnTime - timeReduction, minSpawnTime);
                //let it bring it down by 1ms after it hits "minimum"
                this._lineSpawnTime = (this._lineSpawnTime <= minSpawnTime) ? this._lineSpawnTime - 1 : newSpawnTime;
                this.SpawnLineParticles(args.LineColor, args.CrossedPosition);
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
            var pc = this._gameOver ? Color.White : this.Player.Color;
            //red tints a little darker than cyan, so adjust for that
            var alpha = (byte)(pc.Equals(Color.Red) ? 0x44 : 0x33);
            this._bf.StringSprite.Color = new Color(pc.R, pc.G, pc.B, alpha);
        }

        private void SpawnLineParticles(Color color, Vector2f position)
        {
            //just spawn particles bursting outwards in a uniformly random circle with random velocity
            const float speed = 1.25f;
            var rand = new Random();
            for (int i = 0; i < ParticlesPerCross; i++)
            {
                var angle = rand.NextDouble()*Math.PI*2;
                var vx = (float) Math.Cos(angle) * speed;
                var vy = (float) Math.Sin(angle) * speed;
                var velocity = new Vector2f(vx, vy);
                velocity *= (float)Math.Max(0.2, rand.NextDouble());
                var particle = new Particle(color, position, velocity);
                particle.ParticleFinished += (sender, args) => this._finishedParticles.Enqueue(sender as Particle);
                this._particles.Add(new Particle(color, position, velocity));
            }
        }

        private void UpdateLines()
        {
            foreach (DodgeLine line in this._lines)
                line.Update();

            while (this._finishedLines.Count > 0)
                this._lines.Remove(this._finishedLines.Dequeue());
        }

        private void UpdateParticles()
        {
            foreach(var particle in this._particles)
                particle.Update();

            while (this._finishedParticles.Count > 0)
                this._particles.Remove(this._finishedParticles.Dequeue());
        }
    }
}
