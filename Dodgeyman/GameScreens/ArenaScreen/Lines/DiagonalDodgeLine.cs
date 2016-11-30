namespace Dodgeyman.GameScreens.ArenaScreen.Lines
{
    using System;
    using Code.Extensions;
    using Models;
    using SFML.Graphics;
    using SFML.System;

    class DiagonalDodgeLine : DodgeLine
    {
        //want it to look like it's moving diagonally at 3.5px/frame
        private static readonly Vector2f BaseVelocity = new Vector2f((float)Math.Sqrt(3.5f*3.5f + 3.5f*3.5f), 0);
        private readonly LineShape _lineShape;
        private readonly Vector2u _screenSize;
        private readonly Vector2f _velocity;

        private int _playerSide;

        public DiagonalDodgeLine(Player player, DiagonalDodgeLineDirection direction, Color color) : base(player)
        {
            this._screenSize = GameScreenManager.RenderWindow.Size;

            //lines will be diagonal but move left or right. looks like they're moving diagonally though
            Vector2f linePosition;
            Vector2f offset;
            switch (direction)
            {
                //case DiagonalDodgeLineDirection.BottomLeft: //redundant
                default:
                    linePosition = new Vector2f(-this._screenSize.X, 0);
                    offset = new Vector2f(this._screenSize.X, this._screenSize.Y);
                    this._velocity = BaseVelocity;
                    break;
                case DiagonalDodgeLineDirection.BottomRight:
                    linePosition = new Vector2f(this._screenSize.X * 2, 0);
                    offset = new Vector2f(-this._screenSize.X, this._screenSize.Y);
                    this._velocity = -BaseVelocity;
                    break;
                case DiagonalDodgeLineDirection.TopLeft:
                    linePosition = new Vector2f(0, 0);
                    offset = new Vector2f(-this._screenSize.X, this._screenSize.Y);
                    this._velocity = BaseVelocity;
                    break;
                case DiagonalDodgeLineDirection.TopRight:
                    linePosition = new Vector2f(this._screenSize.X, 0);
                    offset = new Vector2f(this._screenSize.X, this._screenSize.Y);
                    this._velocity = -BaseVelocity;
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
                //if both the x values of the line are outside of the screen then the line is finished
                //give it a few pixels on either side of the screen though
                const float buffer = 5f;
                var width = this._screenSize.X + buffer;
                var x = this._lineShape.GlobalPoint1.X;
                var xx = this._lineShape.GlobalPoint2.X;
                return (x < -buffer && xx < -buffer) || (x > width && xx > width);
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
