namespace Dodgeyman.GameScreens.ArenaScreen.Lines
{
    using System;
    using Code.Extensions;
    using Models;
    using SFML.Graphics;
    using SFML.System;

    internal class DodgeLine : ActiveEntity
    {
        private const float LineWidth = 2f;
        private const float Speed = 3.5f;

        private readonly DodgeLineDirection _direction;
        private readonly RectangleShape _lineShape;
        private readonly Player _player;
        private readonly Vector2u _screenSize;
        private readonly Vector2f _velocity;

        private bool _isCrossed;
        //which side of the line the player is on. starts at 0 then is either 1 or -1 after the first check
        private int _playerSide;

        public event EventHandler<LineCrossedEventArgs> Crossed;
        public event EventHandler Finished;

        public DodgeLine(Vector2u screenSize, Player player)
        {
            this._player = player;
            this._screenSize = screenSize;
            this._direction = GetRandomDirection();

            //initialize position
            Vector2f pos;
            if(this._direction == DodgeLineDirection.Down || this._direction == DodgeLineDirection.Right)
                pos = new Vector2f(0, 0);
            else if(this._direction == DodgeLineDirection.Left)
                pos = new Vector2f(this._screenSize.X - LineWidth, 0);
            else
                pos = new Vector2f(0, this._screenSize.Y - LineWidth);                

            //initialize size and speed
            Vector2f size;
            if (this._direction == DodgeLineDirection.Left || this._direction == DodgeLineDirection.Right)
                size = new Vector2f(LineWidth, this._screenSize.Y);
            else
                size = new Vector2f(this._screenSize.X, LineWidth);

            //initialize velocity
            if(this._direction == DodgeLineDirection.Right)
                this._velocity = new Vector2f(Speed, 0);
            else if (this._direction == DodgeLineDirection.Left)
                this._velocity = new Vector2f(-Speed, 0);
            else if(this._direction == DodgeLineDirection.Down)
                this._velocity = new Vector2f(0, Speed);
            else
                this._velocity = new Vector2f(0, -Speed);

            //pick Color
            var rand = new Random();
            Color c = rand.Next()%2 == 0 ? Color.Cyan : Color.Red;
            this._lineShape = new RectangleShape(size) { Position = pos, FillColor = c };

            //initialize side for collision detection
            this.UpdatePlayerSide();

            this.IsActive = true; //enable it to move and be detected
        }

        // ---------------------------------------------
        // PROPERTIES
        // ---------------------------------------------

        private bool IsFinished
        {
            get
            {
                if (this._direction == DodgeLineDirection.Down && this._lineShape.Position.Y > this._screenSize.Y)
                    return true;
                if (this._direction == DodgeLineDirection.Left && (this._lineShape.Position.X + LineWidth) < 0)
                    return true;
                if (this._direction == DodgeLineDirection.Right && this._lineShape.Position.X > this._screenSize.X)
                    return true;
                if (this._direction == DodgeLineDirection.Up && (this._lineShape.Position.Y + LineWidth) < 0)
                    return true;

                return false;
            }
        }

        // ---------------------------------------------
        // INHERITED MEMBERS
        // ---------------------------------------------

        #region Inherited members

        //ActiveEntity
        protected override void Activate()
        {
            //nothing to do here
        }

        //ActiveEntity
        protected override void Deactivate()
        {
            //nothing to do here
        }

        #endregion Inherited members

        // ---------------------------------------------
        // METHODS
        // ---------------------------------------------

        private static DodgeLineDirection GetRandomDirection()
        {
            Array values = Enum.GetValues(typeof (DodgeLineDirection));
            var random = new Random();
            return (DodgeLineDirection) values.GetValue(random.Next(values.Length));
        }

        public void Update()
        {
            if (!this.IsActive)
                return;

            this._lineShape.Position += this._velocity;
            if (this.IsFinished)
                this.Finished.SafeInvoke(this, EventArgs.Empty);

            // if the line is finished then it shouldn't be collidable
            this.CheckCollision();
        }

        public void Draw(RenderTarget target)
        {
            target.Draw(this._lineShape);
        }

        private void CheckCollision()
        {
            var prevSide = this._playerSide;
            this.UpdatePlayerSide();

            if(this._playerSide != prevSide)
            {
                var collided = !this._lineShape.FillColor.Equals(this._player.Color);
                if(collided || !this._isCrossed) //only count scoring crosses once, count collisions all the time
                    this.Crossed.SafeInvoke(this, new LineCrossedEventArgs(collided));
                this._isCrossed = true;
            }
        }

        private Vector2f GetPosition1()
        {
            if (this._direction == DodgeLineDirection.Down || this._direction == DodgeLineDirection.Up)
                return new Vector2f(0, this._lineShape.Position.Y + LineWidth/2);
            return new Vector2f(this._lineShape.Position.X + LineWidth/2, 0);
        }

        private Vector2f GetPosition2()
        {
            if (this._direction == DodgeLineDirection.Down || this._direction == DodgeLineDirection.Up)
                return new Vector2f(this._screenSize.X, this._lineShape.Position.Y + LineWidth/2);
            return new Vector2f(this._lineShape.Position.X + LineWidth/2, this._screenSize.Y);
        }

        private void UpdatePlayerSide()
        {
            var first = this.GetPosition1();
            var second = this.GetPosition2();
            var offsetToSecond = second - first;
            var offsetToPlayer = this._player.HitPosition - first;
            var crossProduct = offsetToSecond.Cross(offsetToPlayer);
            this._playerSide = crossProduct > 0 ? 1 : -1;
        }
    }
}
