namespace Dodgeyman.GameScreens.ArenaScreen.Lines
{
    using Code.Extensions;
    using Models;
    using SFML.Graphics;
    using SFML.System;

    class DiagonalDodgeLine : DodgeLine
    {
        private const float BaseVelocity = 3.5f;
        private readonly LineShape _lineShape;
        private readonly Vector2u _screenSize;
        private readonly Vector2f _velocity;

        private int _playerSide;

        public DiagonalDodgeLine(Player player, DiagonalDodgeLineDirection direction, Color color) : base(player)
        {
            this._screenSize = GameScreenManager.RenderWindow.Size;

            //get position, offset, and velocity of line
            var halfx = this._screenSize.X/2f;
            var halfy = this._screenSize.Y/2f;
            Vector2f linePosition;
            Vector2f offset;
            switch (direction)
            {
                //case DiagonalDodgeLineDirection.BottomLeft: //redundant
                default:
                    linePosition = new Vector2f(-halfx, halfy);
                    offset = new Vector2f(this._screenSize.X, this._screenSize.Y);
                    this._velocity = new Vector2f(BaseVelocity, -BaseVelocity);
                    break;
                case DiagonalDodgeLineDirection.BottomRight:
                    linePosition = new Vector2f(halfx, this._screenSize.Y * 1.5f);
                    offset = new Vector2f(this._screenSize.X, -this._screenSize.Y);
                    this._velocity = new Vector2f(-BaseVelocity, -BaseVelocity);
                    break;
                case DiagonalDodgeLineDirection.TopLeft:
                    linePosition = new Vector2f(-halfx, halfy);
                    offset = new Vector2f(this._screenSize.X, -this._screenSize.Y);
                    this._velocity = new Vector2f(BaseVelocity, BaseVelocity);
                    break;
                case DiagonalDodgeLineDirection.TopRight:
                    linePosition = new Vector2f(halfx, -halfy);
                    offset = new Vector2f(this._screenSize.X, this._screenSize.Y);
                    this._velocity = new Vector2f(-BaseVelocity, BaseVelocity);
                    break;
            }

            this._lineShape = new LineShape(offset, LineThickness);
            this._lineShape.FillColor = color;
            this._lineShape.Position = linePosition;
            this.FindPlayerSide();
        }

        // ---------------------------------------------
        // PROPERTIES
        // ---------------------------------------------

        private bool IsFinished
        {
            get
            {
                //if the middle of the line is outside of the screen then the line is finished.
                var middle = (this._lineShape.GlobalPoint1 + this._lineShape.GlobalPoint2)/2;
                return (middle.X < 0 || middle.X > this._screenSize.X);
            }
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
            if(this.IsFinished)
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
