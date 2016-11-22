namespace Dodgeyman.GameScreens.ArenaScreen.Lines
{
    using System;
    using Code.Extensions;
    using Models;
    using SFML.Graphics;
    using SFML.System;

    class DiagonalDodgeLine : DodgeLine
    {
        private const float BaseVelocity = 3.5f;
        private readonly LineShape _lineShape;
        private readonly Vector2f _velocity;
        private int _playerSide;

        public DiagonalDodgeLine(Player player) : base(player)
        {
            //this._direction = GetRandomDirection();
            var direction = GetRandomDirection();
            var screenSize = GameScreenManager.RenderWindow.Size;

            //get position(first point), offset(end of line from first point), and velocity
            var linePosition = new Vector2f();
            var offset = new Vector2f();

            //Vector2f velocity;
            var halfx = screenSize.X/2f;
            var halfy = screenSize.Y/2f;
            switch (direction)
            {
                case DiagonalDodgeLineDirection.BottomLeft:
                    linePosition = new Vector2f(-halfx, halfy);
                    offset = new Vector2f(screenSize.X, screenSize.Y);
                    this._velocity = new Vector2f(BaseVelocity, -BaseVelocity);
                    break;
                case DiagonalDodgeLineDirection.BottomRight:
                    linePosition = new Vector2f(halfx, screenSize.Y * 1.5f);
                    offset = new Vector2f(screenSize.X, -screenSize.Y);
                    this._velocity = new Vector2f(-BaseVelocity, -BaseVelocity);
                    break;
                case DiagonalDodgeLineDirection.TopLeft:
                    linePosition = new Vector2f(-halfx, halfy);
                    offset = new Vector2f(screenSize.X, -screenSize.Y);
                    this._velocity = new Vector2f(BaseVelocity, BaseVelocity);
                    break;
                case DiagonalDodgeLineDirection.TopRight:
                    linePosition = new Vector2f(halfx, -halfy);
                    offset = new Vector2f(screenSize.X, screenSize.Y);
                    this._velocity = new Vector2f(-BaseVelocity, BaseVelocity);
                    break;
            }

            var rand = new Random();
            var color = rand.Next()%2 == 0 ? Color.Cyan : Color.Red;
            this._lineShape = new LineShape(offset, 2);
            this._lineShape.FillColor = color;
            this._lineShape.Position = linePosition;
            this.FindPlayerSide();
        }

        // ---------------------------------------------
        // INHERITED MEMBERS
        // ---------------------------------------------

        #region Inherited members

        public override void Dispose()
        {
            this._lineShape.Dispose();
        }

        public override void Draw(RenderTarget target)
        {
            target.Draw(this._lineShape);
        }

        public override void Update()
        {
            if (!this.IsActive)
                return;
            this._lineShape.Position += this._velocity;
            this.CheckCollision();
        }

        #endregion Inherited members

        // ---------------------------------------------
        // METHODS
        // ---------------------------------------------

        private static DiagonalDodgeLineDirection GetRandomDirection()
        {
            Array values = Enum.GetValues(typeof (DiagonalDodgeLineDirection));
            var random = new Random();
            return (DiagonalDodgeLineDirection) values.GetValue(random.Next(values.Length));
        }

        private void CheckCollision()
        {
            if (this.IsCrossed)
                return;

            var prevSide = this._playerSide;
            this.FindPlayerSide();
            if (prevSide == this._playerSide)
                return;

            var collided = !this.Player.Color.Equals(this._lineShape.FillColor);
            this.OnCrossed(new LineCrossedEventArgs(collided, this.Player.HitPosition, this._lineShape.FillColor));
            this.DimShape(this._lineShape);
            this.IsCrossed = true;
        }

        private void FindPlayerSide()
        {
            var offsetToLineEnd = this._lineShape.GlobalPoint2 - this._lineShape.GlobalPoint1;
            var offsetToPlayer = this.Player.HitPosition - this._lineShape.GlobalPoint1;
            var crossProduct = offsetToLineEnd.Cross(offsetToPlayer);
            this._playerSide = crossProduct > 0 ? 1 : -1;
        }
    }
}
