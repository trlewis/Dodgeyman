namespace Dodgeyman.GameScreens.ArenaScreen.Lines
{
    using System;

    static class DodgeLineGenerator
    {
        private static readonly Random Rand = new Random();
        
        //not sure yet if this class will be necessary or not.
        public static DodgeLine GenerateLine(Player player)
        {
            var val = Rand.Next()%2;
            if (val == 0)
                return new OrthagonalDodgeLine(player);
            else
                return new RotateDodgeLine(player);
        }
    }
}
