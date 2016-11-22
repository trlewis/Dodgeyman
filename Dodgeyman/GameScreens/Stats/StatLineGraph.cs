namespace Dodgeyman.GameScreens.Stats
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Globalization;
    using Font;
    using SFML.Graphics;
    using SFML.System;

    class StatLineGraph : IDisposable
    {
        private readonly BitmapFont _bf;
        private readonly RenderTexture _renderTexture;
        private readonly Vector2u _size;
        private readonly IList<int> _values;

        private FloatRect _lineArea;

        public StatLineGraph(Vector2u size, IList<int> values)
        {
            this._bf = new BitmapFont("Assets/5x5numbers.png");
            this._values = values;
            this._size = size;
            this._renderTexture = new RenderTexture(this._size.X, this._size.Y);
            this._renderTexture.Clear(Color.Transparent);
            this.GraphSprite = new Sprite(this._renderTexture.Texture, new IntRect(0, 0, (int)this._size.X, (int)this._size.Y));
            this.DrawAxes();
            this.DrawStats();
            this._renderTexture.Display();
        }

        // ---------------------------------------------
        // PROPERTIES
        // ---------------------------------------------

        public Sprite GraphSprite { get; private set; }

        // ---------------------------------------------
        // INHERITED MEMBERS
        // ---------------------------------------------

        #region Inherited members

        public void Dispose()
        {
            this._bf.Dispose();
            this.GraphSprite.Dispose();
            this._renderTexture.Dispose();
        }

        #endregion Inherited members

        // ---------------------------------------------
        // METHODS
        // ---------------------------------------------

        private void DrawAxes()
        {
            var max = this._values.Max();
            //assuming 0 as min
            int axiswidth = 0;
            this._bf.RenderText(max.ToString(CultureInfo.InvariantCulture));            
            this._bf.StringSprite.Position = new Vector2f(0, 0);
            this._renderTexture.Draw(this._bf.StringSprite);
            if (this._bf.StringSprite.TextureRect.Width > axiswidth)
                axiswidth = this._bf.StringSprite.TextureRect.Width;

            this._bf.RenderText("0");
            var originx = axiswidth - this._bf.StringSprite.TextureRect.Width;
            if (originx < 0)
                originx = 0;
            var originy = this._size.Y - this._bf.StringSprite.TextureRect.Height;
            this._bf.StringSprite.Position = new Vector2f(originx, originy);
            this._renderTexture.Draw(this._bf.StringSprite);
            if (this._bf.StringSprite.TextureRect.Width > axiswidth)
                axiswidth = this._bf.StringSprite.TextureRect.Width;

            //create vertices for the axis lines
            axiswidth += 4;
            var vertices = new Vertex[3];
            vertices[0] = new Vertex(new Vector2f(axiswidth, 0), Color.White);
            vertices[1] = new Vertex(new Vector2f(axiswidth, this._size.Y), Color.White);
            vertices[2] = new Vertex(new Vector2f(this._size.X-1, this._size.Y-1), Color.White);
            this._renderTexture.Draw(vertices, PrimitiveType.LinesStrip);

            this._lineArea = new FloatRect(axiswidth, 0, this._size.X - axiswidth, this._size.Y);
        }

        private void DrawStats()
        {
            var xinc = this._lineArea.Width/(this._values.Count-1);
            var max = this._values.Max();
            var yscale = this._lineArea.Height/max;
            var vertices = new Vertex[this._values.Count];

            var xpos = this._lineArea.Left;
            for (int i = 0; i < this._values.Count; i++)
            {
                var v = this._values[i];
                var ypos = this._lineArea.Height - (v*yscale);
                var diff = v/(float) max;
                var color = InterpolateColor(Color.Red, Color.Cyan, diff);
                vertices[i] = new Vertex(new Vector2f(xpos, ypos), color);
                xpos += xinc;
            }
            this._renderTexture.Draw(vertices, PrimitiveType.LinesStrip);
        }

        private static Color InterpolateColor(Color first, Color second, float diff)
        {
            //maybe not the best way to find the middle of two colors but it'll work for now
            var dr = second.R - first.R;
            var dg = second.G - first.G;
            var db = second.B - first.B;

            var red = (byte) (first.R + (uint)(dr*diff));
            var green = (byte) (first.G + (uint)(dg*diff));
            var blue = (byte) (first.B + (uint)(db*diff));

            return new Color(red, green, blue, 0xFF);
        }
    }
}
