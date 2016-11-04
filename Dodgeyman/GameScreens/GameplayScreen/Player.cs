namespace Dodgeyman.GameScreens.GameplayScreen
{
    using SFML.Graphics;
    using SFML.System;
    using SFML.Window;

    internal class Player
    {
        private const float Speed = 4.0f;
        private Vector2f _velocity;
        private bool _movingLeft;
        private bool _movingRight;
        private bool _movingDown;
        private bool _movingUp;

        public Player(Vector2f initialPosition)
        {
            //this._position = initialPosition;
            this._velocity = new Vector2f(0, 0);
            this.PlayerSprite = new RectangleShape(new Vector2f(3, 3));
            this.PlayerSprite.FillColor = Color.White;
            this.PlayerSprite.Position = initialPosition;
        }

        //public Vector2f Position { get { return this._position; } }
        public Shape PlayerSprite { get; private set; }

        public void Update()
        {
            this.UpdatePosition();
        }

        private void UpdatePosition()
        {
            /* i know this looks weird, but it'll prevent wonky behavior when the user presses
             * left+right or up+down. instead of reversing directions it'll just stop the player
             * and resume the direction they're holding when they let go of one of the keys. */
            float vx = (this._movingLeft ? -1 : 0) + (this._movingRight ? 1 : 0);
            float vy = (this._movingUp ? -1 : 0) + (this._movingDown ? 1 : 0);
            this._velocity = new Vector2f(vx*Speed, vy*Speed);
            this.PlayerSprite.Position += this._velocity;
        }

        public void KeyPressed(object sender, KeyEventArgs e)
        {
            switch (e.Code)
            {
                case Keyboard.Key.Up:
                    this._movingUp = true;
                    break;
                case Keyboard.Key.Down:
                    this._movingDown = true;
                    break;
                case Keyboard.Key.Left:
                    this._movingLeft = true;
                    break;
                case Keyboard.Key.Right:
                    this._movingRight = true;
                    break;
            }
        }

        public void KeyReleased(object sender, KeyEventArgs e)
        {
            switch (e.Code)
            {
                case Keyboard.Key.Up:
                    this._movingUp = false;
                    break;
                case Keyboard.Key.Down:
                    this._movingDown = false;
                    break;
                case Keyboard.Key.Left:
                    this._movingLeft = false;
                    break;
                case Keyboard.Key.Right:
                    this._movingRight = false;
                    break;
            }
        }
    }
}
