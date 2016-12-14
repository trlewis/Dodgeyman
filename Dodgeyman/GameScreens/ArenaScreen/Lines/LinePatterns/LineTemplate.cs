namespace Dodgeyman.GameScreens.ArenaScreen.Lines.LinePatterns
{
    using System;
    using SFML.Graphics;
    using SFML.System;

    /// <summary>
    /// Represents a single line in a LinePattern that can be instantiated at times
    /// relative to when the pattern "starts" on screen. This is a general template that encompasses
    /// all line types. Consiquently, not all properties may be used for each line.
    /// </summary>
    class LineTemplate : ICloneable
    {
        private string _color;
        private string _lineType;
        private string _origin;

        /// <summary>
        /// The color of the line. Needs to be either "cyan" or "red"
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global (used by YAML parser)
        public string Color
        {
            get { return this._color; }
            set { this._color = value.Trim().ToLowerInvariant(); }
        }

        /// <summary>
        /// The line type, currently only supports "orthagonal", "diagonal", and "rotate"
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global (used by YAML parser)
        public string LineType
        {
            get { return this._lineType; }
            set { this._lineType = value.Trim().ToLowerInvariant(); }
        }

        /// <summary>
        /// Only applicable to "rotate" line types. Whether or not the line should rotate clockwise.
        /// value in the YAML must be either "true" or "false". "0" and "1" are not accepted.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global (used by YAML parser)
        public bool IsClockwise { get; set; }

        /// <summary>
        /// Where the line starts on screen, or in the case of a "rotate" line, which corner
        /// of the screen is the center of rotation. Dashes are taken out of the string so you
        /// can type "top-left" or "topleft". But if there is both a vertical and horizontal component
        /// then the vertical one must come first. E.g.: "top-right" is good but "right-top" is not.
        /// Possible components are "top", "bottom", "left", "right"
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global (used by YAML parser)
        public string Origin
        {
            get { return this._origin; }
            set { this._origin = value.Trim().Replace("-", "").ToLowerInvariant(); }
        }

        public object Clone()
        {
            var lt = new LineTemplate
                     {
                         Color = this.Color,
                         Origin = this.Origin,
                         IsClockwise = this.IsClockwise,
                         LineType = this.LineType
                     };
            return lt;
        }

        /// <summary>
        /// Converts the template into an actual DodgeLine object
        /// </summary>
        /// <param name="player">The Player the new DodgeLine object will check for collision against.</param>
        /// <param name="targetSize">The screen size or RenderTarget size of the DodgeLine will be drawn on.</param>
        public DodgeLine CreateLine(Player player, Vector2u targetSize)
        {
            if (this.LineType == "diagonal")
                return this.CreateDiagonalDodgeLine(player, targetSize);
            if (this.LineType == "orthagonal")
                return this.CreateOrthagonalDodgeLine(player, targetSize);
            if (this.LineType == "rotate")
                return this.CreateRotateDodgeLine(player, targetSize);

            throw new Exception(String.Format("Unknown line type: {0}", this.LineType));
        }

        private DodgeLine CreateDiagonalDodgeLine(Player player, Vector2u targetSize)
        {
            var color = this.GetColor();

            DiagonalDodgeLineDirection dir;
            if (this.Origin == "topleft")
                dir = DiagonalDodgeLineDirection.TopLeft;
            else if (this.Origin == "topright")
                dir = DiagonalDodgeLineDirection.TopRight;
            else if (this.Origin == "bottomleft")
                dir = DiagonalDodgeLineDirection.BottomLeft;
            else if (this.Origin == "bottomright")
                dir = DiagonalDodgeLineDirection.BottomRight;
            else
                throw new Exception(String.Format("Unknown or incompatible line origin for Diagonal line: {0}", this.Origin));

            return new DiagonalDodgeLine(player, dir, color, targetSize);
        }

        private DodgeLine CreateOrthagonalDodgeLine(Player player, Vector2u targetSize)
        {
            var color = this.GetColor();

            OrthagonalDodgeLineDirection dir;
            if (this.Origin == "top")
                dir = OrthagonalDodgeLineDirection.Up;
            else if (this.Origin == "bottom")
                dir = OrthagonalDodgeLineDirection.Down;
            else if (this.Origin == "left")
                dir = OrthagonalDodgeLineDirection.Left;
            else if (this.Origin == "right")
                dir = OrthagonalDodgeLineDirection.Right;
            else
                throw new Exception(String.Format("Unknown or incompatible line origin for Orthagonal line: {0}", this.Origin));

            return new OrthagonalDodgeLine(player, dir, color, targetSize);
        }

        private DodgeLine CreateRotateDodgeLine(Player player, Vector2u targetSize)
        {
            var color = this.GetColor();

            RotateDodgeLineCenter center;
            if (this.Origin == "topleft")
                center = RotateDodgeLineCenter.TopLeft;
            else if (this.Origin == "topright")
                center = RotateDodgeLineCenter.TopRight;
            else if (this.Origin == "bottomleft")
                center = RotateDodgeLineCenter.BottomLeft;
            else if (this.Origin == "bottomright")
                center = RotateDodgeLineCenter.BottomRight;
            else
                throw new Exception(String.Format("Unknown or incompatible line origin: {0}", this.Origin));

            return new RotateDodgeLine(player, center, this.IsClockwise, color, targetSize);
        }

        private Color GetColor()
        {
            if (this.Color == "cyan")
                return SFML.Graphics.Color.Cyan;
            if (this.Color == "red")
                return SFML.Graphics.Color.Red;

            throw new Exception(String.Format("Unknown or unsupported color: {0}", this.Color));
        }
    }
}
