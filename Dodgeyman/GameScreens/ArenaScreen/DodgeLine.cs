namespace Dodgeyman.GameScreens.ArenaScreen
{
    using System;
    using SFML.Graphics;
    using SFML.System;

    internal class DodgeLine
    {
        private const float LineWidth = 2f;
        private const float Speed = 3.5f;
        private readonly DodgeLineDirection _direction;
        private readonly Vector2u _screenSize;
        private readonly RectangleShape LineShape;
        private readonly Vector2f _velocity;

        public DodgeLine(Vector2u screenSize)
        {
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

            this.LineShape = new RectangleShape(size);
            this.LineShape.Position = pos;
            this.LineShape.FillColor = c;
        }

        public bool IsFinished
        {
            get
            {
                if (this._direction == DodgeLineDirection.Down && this.LineShape.Position.Y > this._screenSize.Y)
                    return true;
                if (this._direction == DodgeLineDirection.Left && (this.LineShape.Position.X + LineWidth) < 0)
                    return true;
                if (this._direction == DodgeLineDirection.Right && this.LineShape.Position.X > this._screenSize.X)
                    return true;
                if (this._direction == DodgeLineDirection.Up && (this.LineShape.Position.Y + LineWidth) < 0)
                    return true;
                return false;
            }
        }

        private static DodgeLineDirection GetRandomDirection()
        {
            Array values = Enum.GetValues(typeof (DodgeLineDirection));
            var random = new Random();
            return (DodgeLineDirection) values.GetValue(random.Next(values.Length));
        }

        public void Update()
        {
            this.LineShape.Position += this._velocity;
        }

        public void Draw(RenderTarget target)
        {
            target.Draw(this.LineShape);
        }
    }
}
