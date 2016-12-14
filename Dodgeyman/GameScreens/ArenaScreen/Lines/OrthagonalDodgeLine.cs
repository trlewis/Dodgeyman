namespace Dodgeyman.GameScreens.ArenaScreen.Lines
{
    using Code.Extensions;
    using Models;
    using SFML.Graphics;
    using SFML.System;

    /// <summary>
    /// A horizontal or vertical line that sweeps across the screen.
    /// </summary>
    internal class OrthagonalDodgeLine : DodgeLine
    {
        private const float Speed = 3.5f;
        private readonly LineShape _lineShape;
        private readonly Vector2f _velocity;

        //which side of the line the player is on. starts at 0 then is either 1 or -1 after the first check
        private int _playerSide;

        public OrthagonalDodgeLine(Player player, OrthagonalDodgeLineDirection direction, Color color, Vector2u targetSize)
            : base(player, targetSize)
        {
            Vector2f center;
            Vector2f offset;
            switch (direction)
            {
                //case OrthagonalDodgeLineDirection.Left: //redundant
                default:
                    center = new Vector2f(0, 0);
                    offset = new Vector2f(0, this.TargetSize.Y);
                    this._velocity = new Vector2f(Speed, 0);
                    break;
                case OrthagonalDodgeLineDirection.Right: 
                    center = new Vector2f(this.TargetSize.X, 0);
                    offset = new Vector2f(0, this.TargetSize.Y);
                    this._velocity = new Vector2f(-Speed, 0);
                    break;
                case OrthagonalDodgeLineDirection.Top:
                    center = new Vector2f(0, 0);
                    offset = new Vector2f(this.TargetSize.X, 0);
                    this._velocity = new Vector2f(0, Speed);
                    break;
                case OrthagonalDodgeLineDirection.Bottom:
                    center = new Vector2f(0, this.TargetSize.Y);
                    offset = new Vector2f(this.TargetSize.X, 0);
                    this._velocity = new Vector2f(0, -Speed);
                    break;
            }

            this._lineShape = new LineShape(offset, LineThickness) { Position = center, FillColor = color };
            //initialize side for collision detection
            this.UpdatePlayerSide();
        }

        // ---------------------------------------------
        // PROPERTIES
        // ---------------------------------------------

        private bool IsFinished
        {
            get
            {
                var pos = this._lineShape.Position;
                return pos.X < 0 || pos.X > this.TargetSize.X || pos.Y < 0 || pos.Y > this.TargetSize.Y;
            }
        }

        // ---------------------------------------------
        // INHERITED MEMBERS
        // ---------------------------------------------

        #region Inherited members

        // DodgeLine
        public override void Update()
        {
            if (!this.IsActive)
                return;

            this._lineShape.Position += this._velocity;
            if (this.IsFinished)
                this.OnFinished();
            else
                this.CheckCollision();
        }

        // IDisposable
        public override void Dispose()
        {
            this._lineShape.Dispose();
        }

        // DodgeLine
        public override void Draw(RenderTarget target)
        {
            target.Draw(this._lineShape);
        }

        #endregion Inherited members

        // ---------------------------------------------
        // METHODS
        // ---------------------------------------------

        private void CheckCollision()
        {
            if (this.IsCrossed)
                return;

            var prevSide = this._playerSide;
            this.UpdatePlayerSide();
            if (this._playerSide == prevSide) 
                return;

            var collided = !this._lineShape.FillColor.Equals(this.Player.Color);
            this.OnCrossed(new LineCrossedEventArgs(collided, this.Player.HitPosition, this._lineShape.FillColor));
            this.DimShape(this._lineShape);
            this.IsCrossed = true;
        }

        private void UpdatePlayerSide()
        {
            var first = this._lineShape.GlobalPoint1;
            var second = this._lineShape.GlobalPoint2;
            var offsetToSecond = second - first;
            var offsetToPlayer = this.Player.HitPosition - first;
            var crossProduct = offsetToSecond.Cross(offsetToPlayer);
            this._playerSide = crossProduct > 0 ? 1 : -1;
        }
    }
}
