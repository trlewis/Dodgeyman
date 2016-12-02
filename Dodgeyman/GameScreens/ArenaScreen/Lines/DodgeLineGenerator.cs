namespace Dodgeyman.GameScreens.ArenaScreen.Lines
{
    using System;
    using SFML.Graphics;
    using SFML.System;

    static class DodgeLineGenerator
    {
        private static readonly Random Rand = new Random();
        
        //not sure yet if this class will be necessary or not.
        /// <summary>
        /// Randomly generates a DodgeLine object
        /// </summary>
        /// <param name="player">The Player object that the DodgeLine will keep track of.</param>
        /// <param name="targetSize">The screen size or visible area of where the line will be.</param>
        /// <returns></returns>
        public static DodgeLine GenerateLine(Player player, Vector2u targetSize)
        {
            int val = Rand.Next(100);
            if(val < 40)
                return GenerateOrthagonalDodgeLine(player, targetSize);
            if (val < 80)
                return GenerateDiagonalDodgeLine(player, targetSize);

            return GenerateRotateDodgeLine(player, targetSize);
        }

        // ---------------------------------------------
        // METHODS
        // ---------------------------------------------

        private static DiagonalDodgeLine GenerateDiagonalDodgeLine(Player player, Vector2u targetSize)
        {
            Array values = Enum.GetValues(typeof (DiagonalDodgeLineDirection));
            var dir = (DiagonalDodgeLineDirection) values.GetValue(Rand.Next(values.Length));
            return new DiagonalDodgeLine(player, dir, GetRandomColor(), targetSize);
        }

        private static OrthagonalDodgeLine GenerateOrthagonalDodgeLine(Player player, Vector2u targetSize)
        {
            Array values = Enum.GetValues(typeof (OrthagonalDodgeLineDirection));
            var dir = (OrthagonalDodgeLineDirection) values.GetValue(Rand.Next(values.Length));
            return new OrthagonalDodgeLine(player, dir, GetRandomColor(), targetSize);
        }

        private static RotateDodgeLine GenerateRotateDodgeLine(Player player, Vector2u targetSize)
        {
            Array values = Enum.GetValues(typeof (RotateDodgeLineCenter));
            var dir = (RotateDodgeLineCenter) values.GetValue(Rand.Next(values.Length));
            var isClockwise = Rand.Next()%2 == 0;
            return new RotateDodgeLine(player, dir, isClockwise, GetRandomColor(), targetSize);
        }

        private static Color GetRandomColor()
        {
            return Rand.Next()%2 == 0 ? Color.Red : Color.Cyan;
        }

    }
}
