namespace Dodgeyman.GameScreens.ArenaScreen
{
    using System;
    using BitmapFont;
    using SFML.System;
    using SFML.Graphics;

    class GameOverPanel : IDisposable
    {
        private BitmapFont _bf;
        private uint _tint = 1;

        public GameOverPanel()
        {
            this._bf = new BitmapFont("Assets/monochromeSimple.png");
            this._bf.RenderText("Game Over");
            this._bf.StringSprite.Scale = new Vector2f(4, 4);

            var windowSize = GameScreenManager.RenderWindow.Size;
            var textWidth = this._bf.StringSprite.TextureRect.Width * 4;
            var textHeight = this._bf.StringSprite.TextureRect.Height * 4;
            // put the game over prompt 1/4 down the screen, in the middle
            var textPos = new Vector2f((windowSize.X - textWidth) / 2, (windowSize.Y - textHeight) / 4);
            this._bf.StringSprite.Position = textPos;
        }

        // ---------------------------------------------
        // INHERITED MEMBERS
        // ---------------------------------------------

        #region Inherited members

        public void Dispose()
        {
            this._bf.Dispose();
        }

        #endregion Inherited members

        // ---------------------------------------------
        // METHODS
        // ---------------------------------------------

        public void Draw(RenderTarget target)
        {
            target.Draw(this._bf.StringSprite);

            //TODO: draw instructions for "esc = main menu, enter = restart"
        }

        public void Update()
        {
            //update game over tint
            if (this._tint >= 255)
                return;
            
            this._tint += 5 + this._tint/5;
            var tintByte = (byte)(this._tint >= 255 ? 0xFF : this._tint);
            var color = new Color(tintByte, tintByte, tintByte, 0xFF);
            this._bf.StringSprite.Color = color;
        }
    }
}
