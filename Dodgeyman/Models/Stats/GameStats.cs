using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dodgeyman.Models.Stats
{
    using System.ComponentModel;

    [Serializable]
    class GameStats
    {
        private static bool _initialized;
        private static GameStats _instance;

        private GameStats() { }

        public GameStats Instance
        {
            get { return _initialized ? _instance : null; }
        }

        /// <summary>
        /// Needs to be called before trying to use the class
        /// </summary>
        public static void Initialize()
        {
            
        }
    }
}
