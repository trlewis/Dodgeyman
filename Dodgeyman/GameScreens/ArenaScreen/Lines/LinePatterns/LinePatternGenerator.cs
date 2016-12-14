namespace Dodgeyman.GameScreens.ArenaScreen.Lines.LinePatterns
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

    static class LinePatternGenerator
    {
        private const string PatternFile = @"Assets/linepatterns.yaml";

        private static bool _isInitialized;
        private static readonly IList<LinePattern> LinePatterns = new List<LinePattern>();
        private static readonly Random Rand = new Random();

        /// <summary>
        /// Returns a collection of LineTemplate objects from a randomly chosen LinePattern
        /// </summary>
        public static IEnumerable<LineTemplate> GetRandomPatternTemplates()
        {
            if(!_isInitialized)
                throw new Exception("LinePatternGenerator must be initialized before use");

            var i = Rand.Next(LinePatterns.Count);
            //don't return the objects themselves, you don't want them being modified
            foreach (var lt in LinePatterns[i].LineTemplates)
                yield return (LineTemplate)lt.Clone();
        }

        /// <summary>
        /// Loads all the patterns from the Assets/linepatterns.yaml file. Does this once per instance, so if the contents
        /// of that file change, the program needs to be restarted. This can be called multiple times, but only the first
        /// time will actually do anything.
        /// </summary>
        public static void Initialize()
        {
            if (_isInitialized)
                return;
            
            LoadPatterns();
            _isInitialized = true;
        }

        private static void LoadPatterns()
        {
            if (!File.Exists(PatternFile))
                throw new FileNotFoundException("Could not find line patterns file", PatternFile);

            var fileText = File.ReadAllText(PatternFile);
            //text editors like to auto-convert four spaces to tabs.
            fileText = fileText.Replace("\t", "    ");

            using (var input = new StringReader(fileText))
            {
                var builder = new DeserializerBuilder();
                builder.WithNamingConvention(new CamelCaseNamingConvention());
                var deserializer = builder.Build();

                var reader = new Parser(input);
                reader.Expect<StreamStart>();
                while (reader.Accept<DocumentStart>())
                {
                    var pattern = deserializer.Deserialize<LinePattern>(reader);
                    LinePatterns.Add(pattern);
                }
            }
        }
    }
}
