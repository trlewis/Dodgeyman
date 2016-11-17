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

        // ---------------------------------------------
        // PROPERTIES
        // ---------------------------------------------

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

        // ---------------------------------------------
        // INHERITED MEMBERS
        // ---------------------------------------------
        
        #region Inherited members

        public override Vector2f GetPoint(uint index)
        {
            if (index > 4)
                return this._vertices[0].Position;
            return this._vertices[index].Position;
        }

        public override uint GetPointCount()
        {
            return 4;
        }

        #endregion Inherited members

        // ---------------------------------------------
        // METHODS
        // ---------------------------------------------
        
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
    }
}
