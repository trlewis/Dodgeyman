namespace Dodgeyman.GameScreens.ArenaScreen
{
    using System;
    using Code;
    using Font;
    using SFML.System;
    using SFML.Graphics;

    class GameOverPanel : IDisposable
    {
        private readonly BitmapFont _bf;
        private uint _tint = 1;

        public GameOverPanel(Vector2u targetSize)
        {
            this._bf = new BitmapFont(ConfigHelper.GeneralFontLocation);
            this._bf.RenderText("GAME OVER");
            this._bf.StringSprite.Scale = new Vector2f(7, 7);

            // put the game over prompt 1/4 down the screen, in the middle
            var tx = ((int) targetSize.X - this._bf.ScreenSize.X)/2;
            var ty = ((int) targetSize.Y - this._bf.ScreenSize.Y)/4;
            var textPos = new Vector2f(tx, ty);
            this._bf.StringSprite.Position = textPos;
        }

        // ---------------------------------------------
        // INHERITED MEMBERS
        // ---------------------------------------------

        #region Inherited members

        //IDisposable
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
