﻿namespace Dodgeyman.Font
{
    using System;
    using SFML.Graphics;
    using SFML.System;

    public class BitmapFont : IDisposable
    {
        private const int StartPixel = 3;
        private const int NumAsciiChars = 256;

        private readonly Image _sourceImage;
        private Texture _sourceTexture;
        private readonly Color _delimeterColor;
        private readonly Color _maskColor;
        private readonly short _charSpacing;
        private readonly short _lineSpacing;
        private readonly bool _isImageKeyed; //whether or not to apply transparency to background
        private readonly IntRect[] _originalLetterPositions = new IntRect[NumAsciiChars];

        private RenderTexture _renderTexture;

        /// <summary>
        /// Create an instance of a bitmap font. Throws an SFML.LoadingFailedException if the file is not found.
        /// </summary>
        /// <param name="filename">The filename of the bitmap font.</param>
        public BitmapFont(string filename)
        {
            //open as image. parse information. create a sprite. scale the sprite. 
            //draw the sprite onto a drawable. get the texture from that drawable
            this._sourceImage = new Image(filename);
            this._delimeterColor = this._sourceImage.GetPixel(0, 0);
            this._maskColor = this._sourceImage.GetPixel(0, 2);
            var options = this._sourceImage.GetPixel(0, 1);
            this._charSpacing = options.R;
            this._lineSpacing = options.G;
            this._isImageKeyed = options.B > 0;
            this.ProcessImage();
            this._renderTexture = new RenderTexture(1000, this._sourceImage.Size.Y);
            this.StringSprite = new Sprite(this._renderTexture.Texture);
        }

        // ---------------------------------------------
        // PROPERTIES
        // ---------------------------------------------

        /// <summary>
        /// The finished text string that is used to draw.
        /// </summary>
        public Sprite StringSprite { get; private set; }

        /// <summary>
        /// Vertical pixels between lines. This value takes into account the scale of the sprite, so set it before using this value
        /// </summary>
        public int ScreenLineHeight { get { return (int)((this._sourceImage.Size.Y + this._lineSpacing) * this.StringSprite.Scale.Y); } }

        /// <summary>
        /// The actual size of the sprite that will be drawn, in pixels. Takes into account the scale of the sprite.
        /// </summary>
        public Vector2f ScreenSize
        {
            get
            {
                var width = this.StringSprite.Scale.X*this.StringSprite.TextureRect.Width;
                var height = this.StringSprite.Scale.Y*this.StringSprite.TextureRect.Height;
                return new Vector2f(width, height);
            }
        }

        // ---------------------------------------------
        // INHERITED MEMBERS
        // ---------------------------------------------

        #region Inherited members

        //IDisposable
        public void Dispose()
        {
            if(this._renderTexture != null)
                this._renderTexture.Dispose();
            if(this.StringSprite != null)
                this.StringSprite.Dispose();
            if(this._sourceTexture != null)
                this._sourceTexture.Dispose();
            if(this._sourceImage != null)
                this._sourceImage.Dispose();
        }

        #endregion Inherited members

        // ---------------------------------------------
        // METHODS
        // ---------------------------------------------

        /// <summary>
        /// Renders the given string and updates <see cref="StringSprite"/> to contain the string.
        /// </summary>
        /// <param name="str">The text to render.</param>
        public void RenderText(string str)
        {
            var width = this.GetStringWidth(str);
            if (width > this._renderTexture.Size.X)
            { // recreate the rendertexture only if the string would be too long to fit
                this._renderTexture.Dispose();
                this._renderTexture = new RenderTexture(width, this._sourceImage.Size.Y);
                this.StringSprite.Texture = this._renderTexture.Texture;
            }
            //clear the rendertexture so we can put the new string on it
            this._renderTexture.Clear(Color.Transparent);
            
            float xpos = 0; //horizontal position of current letter

            //don't want to create/destroy too many sprites, that takes a lot of resources. so just
            //keep reusing the one until we're done drawing the text
            var letterSprite = new Sprite(this._sourceTexture);
            foreach (char c in str)
            {
                var rekt = this._originalLetterPositions[c];
                letterSprite.TextureRect = rekt;
                letterSprite.Position = new Vector2f(xpos, 0);
                //TODO: don't use character spacing before words or after words. that's what the space character is for
                xpos += rekt.Width + this._charSpacing;
                letterSprite.Draw(this._renderTexture, RenderStates.Default);
            }
            letterSprite.Dispose();
            this._renderTexture.Display(); //"apply" the stuff we drew
            
            //set the rect the sprite grabs from to match the size of the string that was just drawn
            this.StringSprite.TextureRect = new IntRect(0, 0, (int)width, (int)this._sourceImage.Size.Y);
        }

        /// <summary>
        /// Gets how many pixels wide the drawable area needs to be
        /// </summary>
        private uint GetStringWidth(string str)
        {
            if (str.Length <= 0)
                return 0;

            uint width = 0;
            foreach (char c in str)
                width += (uint)this._originalLetterPositions[c].Width;
            
            //TODO: don't use character spacing before words or after words. that's what the space character is for
            //this will at least make sure it only includes spaces BETWEEN the characters in a string. That is, for
            //an n length string, there are n-1 spaces between characters, not n.
            width += (uint) ((str.Length - 1)*this._charSpacing);
            return width;
        }

        private void ProcessImage()
        {
            //open as image. parse information. create a sprite. scale the sprite. 
            //draw the sprite onto a drawable. get the texture from that drawable
            uint left = StartPixel;
            uint count = 0; //used for indexing ascii values

            for (uint x = StartPixel; x < this._sourceImage.Size.X; x++)
            {
                var clr = this._sourceImage.GetPixel(x, 0);
                if (clr.Equals(this._delimeterColor) && count < NumAsciiChars)
                {
                    var ir = new IntRect((int)left, 0, (int)(x - left), (int)this._sourceImage.Size.Y);
                    this._originalLetterPositions[count] = ir;
                    left = x + 1;
                    count++;
                }

                if (count >= NumAsciiChars)
                    break;
            }

            if (this._isImageKeyed)
                this._sourceImage.CreateMaskFromColor(this._maskColor);
            this._sourceTexture = new Texture(this._sourceImage);
        }

    }
}
