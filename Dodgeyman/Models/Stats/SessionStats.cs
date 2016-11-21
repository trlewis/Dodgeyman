namespace Dodgeyman.Models.Stats
{
    using System;

    [Serializable]
    class SessionStats
    {
        public int PixelsMoved { get; set; }
        public int ColorSwitches { get; set; }
        public int Score { get; set; }
    }
}
