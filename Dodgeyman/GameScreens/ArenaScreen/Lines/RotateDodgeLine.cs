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

        public RotateDodgeLine(Player player)
            : base(player)
        {
            var rand = new Random();
            var num = rand.Next(32);
            var screenSize = GameScreenManager.RenderWindow.Size;
            uint length = Math.Max(screenSize.X, screenSize.Y);
            length *= 2;

            //pick random center of rotation
            var centerX = num % 2 == 0 ? 0 : GameScreenManager.RenderWindow.Size.X;
            var centerY = num % 4 >= 2 ? 0 : GameScreenManager.RenderWindow.Size.Y;
            //pick CW or CCW (multiplier for base angular velocity)
            var isClockwise = num%8 >= 4;
            this._angularVelocity = isClockwise ? BaseAngularVelocity : -BaseAngularVelocity;
            //pick a color
            var color = num%16 >= 8 ? Color.Red : Color.Cyan;

            //find offset of end point
            var offset = new Vector2f(0, 0);
            if(centerX == 0 && centerY == 0) //top-left
                offset = new Vector2f(isClockwise ? length : 0, isClockwise ? 0 : length);
            else if(centerX == 0 && centerY != 0) //bottom-left
                offset = new Vector2f(isClockwise ? 0 : length, isClockwise ? -length : 0);
            else if(centerX != 0 && centerY == 0) //top-right
                offset = new Vector2f(isClockwise ? 0 : -length, isClockwise ? length : 0);
            else if(centerX != 0 && centerY != 0) //bottom-right
                offset = new Vector2f(isClockwise ? -length : 0, isClockwise ? 0 : -length);

            this._lineShape = new LineShape(offset, LineThickness);
            this._lineShape.Position = new Vector2f(centerX, centerY);
            this._lineShape.FillColor = color;
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

        // DodgeLine
        public override void Update()
        {
            if (!this.IsActive)
                return;

            this._lineShape.Rotation += this._angularVelocity;
            this.CheckCollision();

            if(Math.Abs(this._lineShape.Rotation) > 90)
                this.OnFinished();
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
            this.OnCrossed(new LineCrossedEventArgs(collided));
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
