namespace Dodgeyman.GameScreens.ArenaScreen.Lines
{
    class LineCrossedEventArgs
    {
        public LineCrossedEventArgs(bool wasHit)
        {
            this.Hit = wasHit;
        }

        /// <summary>
        /// If the line crossing was a killing collision
        /// </summary>
        public bool Hit { get; private set; }
    }
}
