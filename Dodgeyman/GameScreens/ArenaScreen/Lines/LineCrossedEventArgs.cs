namespace Dodgeyman.GameScreens.ArenaScreen.Lines
{
    using SFML.Graphics;
    using SFML.System;

    class LineCrossedEventArgs
    {
        public LineCrossedEventArgs(bool wasHit, Vector2f crossedPosition, Color lineColor)
        {
            this.Hit = wasHit;
            this.CrossedPosition = crossedPosition;
            this.LineColor = lineColor;
        }

        /// <summary>
        /// Where the collision occurrerd on screen
        /// </summary>
        public Vector2f CrossedPosition { get; private set; }

        /// <summary>
        /// If the line crossing was a killing collision
        /// </summary>
        public bool Hit { get; private set; }

        /// <summary>
        /// The color ofe the line that was crossed
        /// </summary>
        public Color LineColor { get; private set; }
    }
}
