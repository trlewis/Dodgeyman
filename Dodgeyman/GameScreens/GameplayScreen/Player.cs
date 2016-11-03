using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dodgeyman.GameScreens.GameplayScreen
{
    using SFML.Graphics;
    using SFML.System;
    using SFML.Window;

    internal class Player
    {
        //private Vector2f _position;
        private Vector2f _velocity;
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

        public void UpdatePosition()
        {
            //this._position += this._velocity;
            this.PlayerSprite.Position += this._velocity;
        }

        public void KeyPressed(object sender, KeyEventArgs e)
        {
            switch (e.Code)
            {
                case Keyboard.Key.Up:
                    this._velocity.Y = -4;
                    break;
                case Keyboard.Key.Down:
                    this._velocity.Y = 4;
                    break;
                case Keyboard.Key.Left:
                    this._velocity.X = -4;
                    break;
                case Keyboard.Key.Right:
                    this._velocity.X = 4;
                    break;
            }
        }

        public void KeyReleased(object sender, KeyEventArgs e)
        {
            switch (e.Code)
            {
                case Keyboard.Key.Up:
                    this._velocity.Y = 0;
                    break;
                case Keyboard.Key.Down:
                    this._velocity.Y = 0;
                    break;
                case Keyboard.Key.Left:
                    this._velocity.X = 0;
                    break;
                case Keyboard.Key.Right:
                    this._velocity.X = 0;
                    break;
            }
        }
    }
}
