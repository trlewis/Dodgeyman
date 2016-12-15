namespace Dodgeyman.Code
{
    using System;
    using System.Configuration;

    /// <summary>
    /// A class that reads info from the App.config file. Use this class to access values
    /// in that file to avoid mistyping strings.
    /// </summary>
    public static class ConfigHelper
    {
        /// <summary>
        /// The all purpose font. Includes spaces for descenders (q, p, j, g, etc)
        /// </summary>
        public static string GeneralFontLocation
        {
            get { return ConfigurationManager.AppSettings["GeneralFontLocation"]; }
        }

        /// <summary>
        /// Contains the digits 0-9 only. Does not include space for descenders, so it is
        /// easier to center vertically.
        /// </summary>
        public static string NumberFontLocation
        {
            get { return ConfigurationManager.AppSettings["NumberFontLocation"]; }
        }

        /// <summary>
        /// What percent of the lines that spawn should be patterns versus single lines.
        /// </summary>
        public static int PatternPercentage
        {
            get { return Int32.Parse(ConfigurationManager.AppSettings["PatternPercentage"]); }
        }

        /// <summary>
        /// The resolution of both dimensions of the window.
        /// </summary>
        public static uint ScreenSize
        {
            get { return UInt32.Parse(ConfigurationManager.AppSettings["ScreenSize"]); }
        }

        /// <summary>
        /// This Game's version number.
        /// </summary>
        public static string VersionNumber
        {
            get { return ConfigurationManager.AppSettings["VersionNumber"]; }
        }
    }
}
