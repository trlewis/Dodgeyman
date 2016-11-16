namespace Dodgeyman.Models
{
    using Code.Extensions;
    using SFML.Graphics;
    using SFML.System;

    class LineShape : Shape
    {
        private readonly Vertex[] _vertices = new Vertex[4];
        private float _thickness;
        private Vector2f _offset;

        public LineShape(Vector2f offset, float thickness)
        {
            this._offset = offset;
            this._thickness = thickness;
            this.CalculateVertices();
        }

        /// <summary>
        /// The offset of the second point from the first. LineShapes are always 
        /// created with the first point at 0,0.
        /// </summary>
        public Vector2f Offset
        {
            get { return this._offset; }
            set
            {
                this._offset = value;
                this.CalculateVertices();
            }
        }

        public float Thickness
        {
            get { return this._thickness; }
            set
            {
                this._thickness = value;
                this.CalculateVertices();
            }
        }

        private void CalculateVertices()
        {
            //vertices are based in local space. that means point 1 is always the origin. point 2 is just the offset from that
            var perp = new Vector2f(this.Offset.Y, -this.Offset.X).Normalize();
            var half = this.Thickness/2;
            this._vertices[0] = new Vertex(new Vector2f(0 + (perp.X*half), 0 + (perp.Y*half)));
            this._vertices[1] = new Vertex(new Vector2f(0 - (perp.X*half), 0 - (perp.Y*half)));
            this._vertices[2] = new Vertex(new Vector2f(this.Offset.X - (perp.X*half), this.Offset.Y - (perp.Y*half)));
            this._vertices[3] = new Vertex(new Vector2f(this.Offset.X + (perp.X*half), this.Offset.Y + (perp.Y*half)));

            this.Update();
        }

        #region Inherited members

        public override uint GetPointCount()
        {
            return 4;
        }

        public override Vector2f GetPoint(uint index)
        {
            if (index > 4)
                return this._vertices[0].Position;
            return this._vertices[index].Position;
        }

        #endregion Inherited members
    }
}
