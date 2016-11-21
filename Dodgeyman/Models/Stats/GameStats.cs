namespace Dodgeyman.Models.Stats
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    [Serializable]
    class GameStats
    {
        private const string FileName = @".\stats.bin";
        private const string TempFileName = @".\stats.bin.tmp";
        private static bool _initialized;
        private static GameStats _instance;

        private GameStats()
        {
            this.SessionStats = new List<SessionStats>();
        }

        #region Singleton code

        /// <summary>
        /// The instance of the GameStats class the program uses. Initialize must be called before
        /// this will return a non-null value.
        /// </summary>
        public static GameStats Instance
        {
            get { return _initialized ? _instance : null; }
        }

        /// <summary>
        /// Needs to be called before trying to use the Instance field. will not re-initialize if it is
        /// already initialized, so feel free to call this method wherever you might use the singleton.
        /// </summary>
        public static void Initialize()
        {
            if (_initialized)
                return;

            if (File.Exists(FileName))
            {
                var formatter = new BinaryFormatter();
                using (var stream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    _instance = (GameStats) formatter.Deserialize(stream);
                }
            }
            else
            {
                _instance = new GameStats();
            }

            _initialized = true;
        }

        public static void WriteStats()
        {
            //remove old temp file if one exists
            if(File.Exists(TempFileName))
                File.Delete(TempFileName);

            //write temp file
            var formatter = new BinaryFormatter();
            using (var stream = new FileStream(TempFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, _instance);
            }

            //rename temp file to permanent file
            if (File.Exists(TempFileName))
            {
                //remove old stats file
                if(File.Exists(FileName))
                    File.Delete(FileName);
                //rename file
                File.Move(TempFileName, FileName);
            }
        }

        #endregion Singleton code

        // ---------------------------------------------
        // PROPERTIES
        // ---------------------------------------------
        public double AverageScore { get; private set; }

        public int GamesPlayed { get; private set; }

        public bool HasStats { get { return this.SessionStats.Count > 0; } }

        public int HighScore { get; private set; }

        public int TotalColorSwitches { get; private set; }

        public int TotalPixelsMoved { get; private set; }

        public int TotalScore { get; private set; }

        /// <summary>
        /// used internally to get other specific stats.
        /// </summary>
        private IList<SessionStats> SessionStats { get; set; }

        // ---------------------------------------------
        // METHODS
        // ---------------------------------------------
        public void AddSessionStats(SessionStats stats)
        {
            if (stats == null)
                return;
            this.SessionStats.Add(stats); //should there be any validation?
            this.CalculateStats();
        }

        private void CalculateStats()
        {
            this.GamesPlayed = this.SessionStats.Count;
            this.HighScore = this.SessionStats.Max(si => si.Score);
            this.AverageScore = this.SessionStats.Average(si => si.Score);
            this.TotalScore = this.SessionStats.Sum(si => si.Score);
            this.TotalColorSwitches = this.SessionStats.Sum(si => si.ColorSwitches);
            this.TotalPixelsMoved = this.SessionStats.Sum(si => si.PixelsMoved);
        }
    }
}
