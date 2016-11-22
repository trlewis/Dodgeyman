namespace Dodgeyman.GameScreens.ArenaScreen
{
    using System;
    using Code.Extensions;
    using Models;
    using SFML.Graphics;
    using SFML.System;
    using SFML.Window;

    internal class Player : ActiveEntity, IDisposable
    {
        private const float PlayerSize = 3f;
        private const float Speed = 2.25f;

        private readonly FloatRect _arenaBounds;

        private bool _movingDown;
        private bool _movingLeft;
        private bool _movingRight;
        private bool _movingUp;
        private Vector2f _velocity;

        public event EventHandler ColorChanged;
        public event EventHandler<int> PlayerMoved;

        public Player(FloatRect arenaBounds)
        {
            this._arenaBounds = arenaBounds;
            this._velocity = new Vector2f(0, 0);
            this.PlayerSprite = new RectangleShape(new Vector2f(PlayerSize, PlayerSize));
            this.PlayerSprite.FillColor = Color.Cyan;
            var px = (arenaBounds.Width/2) + arenaBounds.Left;
            var py = (arenaBounds.Height/2) + arenaBounds.Top;
            this.PlayerSprite.Position = new Vector2f(px, py);
            this.IsActive = true;
        }

        // ---------------------------------------------
        // INHERITED MEMBERS
        // ---------------------------------------------

        #region Inherited members
        
        // IDisposable
        public void Dispose()
        {
            this.PlayerSprite.Dispose();
        }

        // ActiveEntity
        protected override void Activate()
        {
            GameScreenManager.RenderWindow.KeyPressed += this.KeyPressed;
            GameScreenManager.RenderWindow.KeyReleased += this.KeyReleased;
        }

        // ActiveEntity
        protected override void Deactivate()
        {
            GameScreenManager.RenderWindow.KeyPressed -= this.KeyPressed;
            GameScreenManager.RenderWindow.KeyReleased -= this.KeyReleased;
        }

        #endregion Inherited members

        // ---------------------------------------------
        // PROPERTIES
        // ---------------------------------------------

        public Color Color { get { return this.PlayerSprite.FillColor; } }

        /// <summary>
        /// The position that should be used for collision detection
        /// </summary>
        public Vector2f HitPosition
        {
            get
            {
                return new Vector2f(this.PlayerSprite.Position.X + PlayerSize/2, 
                    this.PlayerSprite.Position.Y + PlayerSize/2);
            }
        }

        public RectangleShape PlayerSprite { get; private set; }

        private float PlayerBottom { get { return this.PlayerSprite.Position.Y + this.PlayerSprite.Size.Y; } }
        private float PlayerLeft { get { return this.PlayerSprite.Position.X; } }
        private float PlayerRight { get { return this.PlayerSprite.Position.X + this.PlayerSprite.Size.X; } }
        private float PlayerTop { get { return this.PlayerSprite.Position.Y; } }


        // ---------------------------------------------
        // METHODS
        // ---------------------------------------------

        public void Update()
        {
            if (!this.IsActive)
                return;
            this.UpdatePosition();
        }

        private void KeyPressed(object sender, KeyEventArgs e)
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
                case Keyboard.Key.Z:
                    this.ToggleColor();
                    break;
            }
        }

        private void KeyReleased(object sender, KeyEventArgs e)
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

        private void ToggleColor()
        {
            if (!this.IsActive)
                return;

            this.PlayerSprite.FillColor = this.PlayerSprite.FillColor.Equals(Color.Cyan) ? Color.Red : Color.Cyan;
            this.ColorChanged.SafeInvoke(this, EventArgs.Empty);
        }

        private void UpdatePosition()
        {
            /* i know this looks weird, but it'll prevent wonky behavior when the user presses
             * left+right or up+down. instead of reversing directions it'll just stop the player
             * and resume the direction they're holding when they let go of one of the keys. */
            var oldPosition = new Vector2f(this.PlayerSprite.Position.X, this.PlayerSprite.Position.Y);
            float vx = (this._movingLeft ? -1 : 0) + (this._movingRight ? 1 : 0);
            float vy = (this._movingUp ? -1 : 0) + (this._movingDown ? 1 : 0);
            this._velocity = new Vector2f(vx*Speed, vy*Speed);
            this.PlayerSprite.Position += this._velocity;

            //keep player inside arena. Might want to create some extensions or custom classes/structs to handle all
            //this adding properties together stuff. i'd rather say if player.right > arenabounds.right
            var pos = this.PlayerSprite.Position;
            if (this.PlayerRight > this._arenaBounds.Left + this._arenaBounds.Width)
                pos.X = this._arenaBounds.Left + this._arenaBounds.Width - this.PlayerSprite.Size.X;
            else if (this.PlayerLeft < this._arenaBounds.Left)
                pos.X = this._arenaBounds.Left;

            if (this.PlayerBottom > this._arenaBounds.Top + this._arenaBounds.Height)
                pos.Y = this._arenaBounds.Top + this._arenaBounds.Height - this.PlayerSprite.Size.Y;
            else if (this.PlayerTop < this._arenaBounds.Top)
                pos.Y = this._arenaBounds.Top;
            this.PlayerSprite.Position = pos;

            if (oldPosition != this.PlayerSprite.Position)
            {
                var px = (int) ((oldPosition - this.PlayerSprite.Position).Magnitude());
                this.PlayerMoved.SafeInvoke(this, px);
            }
        }
    }
}
