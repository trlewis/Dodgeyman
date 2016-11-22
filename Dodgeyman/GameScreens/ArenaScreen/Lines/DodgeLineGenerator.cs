namespace Dodgeyman.GameScreens.ArenaScreen.Lines
{
    using System;
    using SFML.Graphics;

    static class DodgeLineGenerator
    {
        private static readonly Random Rand = new Random();
        
        //not sure yet if this class will be necessary or not.
        public static DodgeLine GenerateLine(Player player)
        {
            int val = Rand.Next(100);
            if (val < 33)
                return GenerateOrthagonalDodgeLine(player);
            if (val < 66)
                return GenerateRotateDodgeLine(player);
            if (val < 100)
                return GenerateDiagonalDodgeLine(player);

            return GenerateOrthagonalDodgeLine(player);
        }

        // ---------------------------------------------
        // METHODS
        // ---------------------------------------------

        private static DiagonalDodgeLine GenerateDiagonalDodgeLine(Player player)
        {
            Array values = Enum.GetValues(typeof (DiagonalDodgeLineDirection));
            var dir = (DiagonalDodgeLineDirection) values.GetValue(Rand.Next(values.Length));
            return new DiagonalDodgeLine(player, dir, GetRandomColor());
        }

        private static OrthagonalDodgeLine GenerateOrthagonalDodgeLine(Player player)
        {
            Array values = Enum.GetValues(typeof (OrthagonalDodgeLineDirection));
            var dir = (OrthagonalDodgeLineDirection) values.GetValue(Rand.Next(values.Length));
            return new OrthagonalDodgeLine(player, dir, GetRandomColor());
        }

        private static RotateDodgeLine GenerateRotateDodgeLine(Player player)
        {
            Array values = Enum.GetValues(typeof (RotateDodgeLineCenter));
            var dir = (RotateDodgeLineCenter) values.GetValue(Rand.Next(values.Length));
            var isClockwise = Rand.Next()%2 == 0;
            return new RotateDodgeLine(player, dir, isClockwise, GetRandomColor());
        }

        private static Color GetRandomColor()
        {
            return Rand.Next()%2 == 0 ? Color.Red : Color.Cyan;
        }

    }
}
