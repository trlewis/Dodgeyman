namespace Dodgeyman.GameScreens.ArenaScreen.Lines
{
    using System;

    static class DodgeLineGenerator
    {
        private static readonly Random Rand = new Random();
        
        //not sure yet if this class will be necessary or not.
        public static DodgeLine GenerateLine(Player player)
        {
            //TODO: use the random generator of this class to pick colors/sides for the lines
            var val = Rand.Next() % 3;
            if (val == 0)
                return new OrthagonalDodgeLine(player);
            if(val == 1)
                return new RotateDodgeLine(player);
            if(val == 2)
                return new DiagonalDodgeLine(player);

            return new OrthagonalDodgeLine(player);
        }
    }
}
