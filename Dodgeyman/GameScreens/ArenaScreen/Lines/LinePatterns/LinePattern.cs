namespace Dodgeyman.GameScreens.ArenaScreen.Lines.LinePatterns
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a collection of LineTemplates
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    class LinePattern
    {
        /// <summary>
        /// The name of the pattern. Not parsed or used for decision logic.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public string Name { get; set; }


        /// <summary>
        /// The individual lines that make up this pattern.
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public IList<LineTemplate> LineTemplates { get; set; }
    }
}
