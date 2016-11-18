namespace Dodgeyman.GameScreens.ArenaScreen.Lines
{
    using System;
    using Code.Extensions;
    using SFML.Graphics;
    using SFML.System;

    /// <summary>
    /// A horizontal or vertical line that sweeps across the screen.
    /// </summary>
    internal class OrthagonalDodgeLine : DodgeLine
    {
        private const float Speed = 3.5f;

        private readonly OrthagonalDodgeLineDirection _direction;
        private readonly RectangleShape _lineShape;
        private readonly Vector2u _screenSize;
        private readonly Vector2f _velocity;

        //which side of the line the player is on. starts at 0 then is either 1 or -1 after the first check
        private int _playerSide;

        public OrthagonalDodgeLine(Player player)
            : base(player)
        {
            this._screenSize = GameScreenManager.RenderWindow.Size;
            this._direction = GetRandomDirection();

            //initialize position
            Vector2f pos;
            if(this._direction == OrthagonalDodgeLineDirection.Down || this._direction == OrthagonalDodgeLineDirection.Right)
                pos = new Vector2f(0, 0);
            else if(this._direction == OrthagonalDodgeLineDirection.Left)
                pos = new Vector2f(this._screenSize.X - LineThickness, 0);
            else
                pos = new Vector2f(0, this._screenSize.Y - LineThickness);                

            //initialize size and speed
            Vector2f size;
            if (this._direction == OrthagonalDodgeLineDirection.Left || this._direction == OrthagonalDodgeLineDirection.Right)
                size = new Vector2f(LineThickness, this._screenSize.Y);
            else
                size = new Vector2f(this._screenSize.X, LineThickness);

            //initialize velocity
            if(this._direction == OrthagonalDodgeLineDirection.Right)
                this._velocity = new Vector2f(Speed, 0);
            else if (this._direction == OrthagonalDodgeLineDirection.Left)
                this._velocity = new Vector2f(-Speed, 0);
            else if(this._direction == OrthagonalDodgeLineDirection.Down)
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
                if (this._direction == OrthagonalDodgeLineDirection.Down && this._lineShape.Position.Y > this._screenSize.Y)
                    return true;
                if (this._direction == OrthagonalDodgeLineDirection.Left && (this._lineShape.Position.X + LineThickness) < 0)
                    return true;
                if (this._direction == OrthagonalDodgeLineDirection.Right && this._lineShape.Position.X > this._screenSize.X)
                    return true;
                if (this._direction == OrthagonalDodgeLineDirection.Up && (this._lineShape.Position.Y + LineThickness) < 0)
                    return true;

                return false;
            }
        }

        // ---------------------------------------------
        // INHERITED MEMBERS
        // ---------------------------------------------

        #region Inherited members

        public override void Update()
        {
            if (!this.IsActive)
                return;

            this._lineShape.Position += this._velocity;
            if (this.IsFinished)
                this.OnFinished();

            // if the line is finished then it shouldn't be collidable
            this.CheckCollision();
        }

        public override void Draw(RenderTarget target)
        {
            target.Draw(this._lineShape);
        }

        #endregion Inherited members

        // ---------------------------------------------
        // METHODS
        // ---------------------------------------------

        private static OrthagonalDodgeLineDirection GetRandomDirection()
        {
            Array values = Enum.GetValues(typeof (OrthagonalDodgeLineDirection));
            var random = new Random();
            return (OrthagonalDodgeLineDirection) values.GetValue(random.Next(values.Length));
        }

        private void CheckCollision()
        {
            if (this.IsCrossed)
                return;

            var prevSide = this._playerSide;
            this.UpdatePlayerSide();
            if (this._playerSide == prevSide) 
                return;

            var collided = !this._lineShape.FillColor.Equals(this.Player.Color);
            this.OnCrossed(new LineCrossedEventArgs(collided));
            this.DimShape(this._lineShape);
            this.IsCrossed = true;
        }

        private Vector2f GetPosition1()
        {
            if (this._direction == OrthagonalDodgeLineDirection.Down || this._direction == OrthagonalDodgeLineDirection.Up)
                return new Vector2f(0, this._lineShape.Position.Y + LineThickness/2);
            return new Vector2f(this._lineShape.Position.X + LineThickness/2, 0);
        }

        private Vector2f GetPosition2()
        {
            if (this._direction == OrthagonalDodgeLineDirection.Down || this._direction == OrthagonalDodgeLineDirection.Up)
                return new Vector2f(this._screenSize.X, this._lineShape.Position.Y + LineThickness/2);
            return new Vector2f(this._lineShape.Position.X + LineThickness/2, this._screenSize.Y);
        }

        private void UpdatePlayerSide()
        {
            var first = this.GetPosition1();
            var second = this.GetPosition2();
            var offsetToSecond = second - first;
            var offsetToPlayer = this.Player.HitPosition - first;
            var crossProduct = offsetToSecond.Cross(offsetToPlayer);
            this._playerSide = crossProduct > 0 ? 1 : -1;
        }
    }
}
