namespace Dodgeyman.GameScreens.ArenaScreen
{
    using System;
    using System.Collections.Generic;
    using Code;
    using Font;
    using Models.Stats;
    using SFML.System;
    using SFML.Graphics;

    class GameOverPanel : IDisposable
    {
        private readonly List<string> _actionStrings = new List<string>
                                                       {
                                                           "ESC: QUIT TO MAIN MENU",
                                                           "ENTER: PLAY AGAIN"
                                                       };
        private readonly BitmapFont _bf;
        private readonly Vector2u _targetSize;
        private readonly IList<string> _statStrings = new List<string>();
        private uint _tint = 1;

        public GameOverPanel(Vector2u targetSize, SessionStats sessionStats)
        {
            this._targetSize = targetSize;
            this._bf = new BitmapFont(ConfigHelper.GeneralFontLocation);

            this._statStrings.Add(string.Format("PIXELS MOVED: {0}", sessionStats.PixelsMoved));
            this._statStrings.Add(string.Format("COLOR SWITCHES: {0}", sessionStats.ColorSwitches));
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
            this.DrawGameOverMessage(target);
            this.DrawStats(target);
            this.DrawActions(target);
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

        private void DrawActions(RenderTarget target)
        {
            this._bf.StringSprite.Scale = new Vector2f(2, 2);
            var y = this._targetSize.Y - this._bf.ScreenLineHeight;
            foreach (var action in this._actionStrings)
            {
                this._bf.RenderText(action);
                this._bf.StringSprite.Position = new Vector2f(10, y);
                target.Draw(this._bf.StringSprite);
                y -= this._bf.ScreenLineHeight;
            }
        }

        private void DrawGameOverMessage(RenderTarget target)
        {
            this._bf.RenderText("GAME OVER");
            this._bf.StringSprite.Scale = new Vector2f(7, 7);

            // put the game over prompt 1/4 down the screen, in the middle
            var tx = ((int) this._targetSize.X - this._bf.ScreenSize.X)/2;
            var ty = ((int) this._targetSize.Y - this._bf.ScreenSize.Y)/4;
            var textPos = new Vector2f(tx, ty);
            this._bf.StringSprite.Position = textPos;
            target.Draw(this._bf.StringSprite);
        }

        private void DrawStats(RenderTarget target)
        {
            this._bf.StringSprite.Scale = new Vector2f(3, 3);
            var y = (int)this._targetSize.Y/1.5f;
            foreach (var statLine in this._statStrings)
            {
                this._bf.RenderText(statLine);
                var x = (this._targetSize.X - this._bf.ScreenSize.X)/2;
                this._bf.StringSprite.Position = new Vector2f(x, y);
                target.Draw(this._bf.StringSprite);
                y += this._bf.ScreenLineHeight;
            }
        }
    }
}
