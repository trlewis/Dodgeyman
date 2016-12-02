namespace Dodgeyman.GameScreens.ArenaScreen.Lines
{
    using System;
    using Code.Extensions;
    using Models;
    using SFML.Graphics;
    using SFML.System;

    class RotateDodgeLine : DodgeLine
    {
        private const float BaseAngularVelocity = 0.5f;
        private readonly LineShape _lineShape;
        private readonly float _angularVelocity;

        private int _playerSide;

        public RotateDodgeLine(Player player, RotateDodgeLineCenter center, Boolean isClockwise, Color color, Vector2u targetSize)
            : base(player, targetSize)
        {
            //technically needs to be as long as the magnitude of <target x, target y> but this is close enough.
            uint length = (Math.Max(this.TargetSize.X, this.TargetSize.Y)) * 2;

            Vector2f rotatePoint;
            Vector2f offset;
            switch (center)
            {
                //case RotateDodgeLineCenter.TopLeft: //redundant
                default:
                    rotatePoint = new Vector2f(0, 0);
                    offset = new Vector2f(isClockwise ? length : 0, isClockwise ? 0 : length);
                    break;
                case RotateDodgeLineCenter.BottomLeft:
                    rotatePoint = new Vector2f(0, this.TargetSize.Y);
                    offset = new Vector2f(isClockwise ? 0 : length, isClockwise ? -length : 0);
                    break;
                case RotateDodgeLineCenter.BottomRight:
                    rotatePoint = new Vector2f(this.TargetSize.X, this.TargetSize.Y);
                    offset = new Vector2f(isClockwise ? -length : 0, isClockwise ? 0 : -length);
                    break;
                case RotateDodgeLineCenter.TopRight:
                    rotatePoint = new Vector2f(this.TargetSize.X, 0);
                    offset = new Vector2f(isClockwise ? 0 : -length, isClockwise ? length : 0);
                    break;
            }

            this._lineShape = new LineShape(offset, LineThickness) { Position = rotatePoint, FillColor = color };
            this._angularVelocity = isClockwise ? BaseAngularVelocity : -BaseAngularVelocity;
            this.FindPlayerSide();
        }

        // ---------------------------------------------
        // INHERITED MEMBERS
        // ---------------------------------------------

        #region Inherited Members

        // DodgeLine
        public override void Draw(RenderTarget target)
        {
            target.Draw(this._lineShape);
        }

        // IDisposable
        public override void Dispose()
        {
            this._lineShape.Dispose();
        }

        // DodgeLine
        public override void Update()
        {
            if (!this.IsActive)
                return;

            this._lineShape.Rotation += this._angularVelocity;

            if(Math.Abs(this._lineShape.Rotation) > 90)
                this.OnFinished();
            else
                this.CheckCollision();
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
