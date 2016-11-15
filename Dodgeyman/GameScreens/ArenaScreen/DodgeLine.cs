namespace Dodgeyman.GameScreens.ArenaScreen
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
        private readonly Vector2u _screenSize;
        private readonly RectangleShape _lineShape;
        private readonly Player _player;
        private readonly Vector2f _velocity;

        //which side of the line the player is on. starts at 0 then is either 1 or -1 after the first check
        private int _playerSide;

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

            this.IsActive = true; //enable it to move and be detected
        }

        #region Inherited members

        protected override void Activate()
        {
            //nothing to do here
        }

        protected override void Deactivate()
        {
            //nothing to do here
        }

        #endregion Inherited members

        public bool IsFinished
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

        /// <summary>
        /// Has the player crossed the line, may or may not indicate a collision occured
        /// </summary>
        public bool IsCrossed { get; private set; }

        /// <summary>
        /// Has the player collieded with the line (i.e.: has the player crossed over the line
        /// with a different color?)
        /// </summary>
        public bool IsCollided { get; private set; }

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
            this.CheckCollision();
        }

        public void Draw(RenderTarget target)
        {
            target.Draw(this._lineShape);
        }

        private void CheckCollision()
        {
            //if a collision has already been detected or is inactive don't bother doing anything else
            if (this.IsCollided)
                return;

            //get which side of the line the player is on
            var first = this.GetPosition1();
            var second = this.GetPosition2();
            var offsetToSecond = second - first;
            var offsetToPlayer = this._player.HitPosition - first;

            var cross = offsetToSecond.Cross(offsetToPlayer);
            var side = cross > 0 ? 1 : -1;
            if (this._playerSide != 0 && this._playerSide != side) //don't count the first frame
                this.IsCrossed = true;
            if (this.IsCrossed && !this._lineShape.FillColor.Equals(this._player.Color))
                this.IsCollided = true;
            this._playerSide = side;
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
    }
}
