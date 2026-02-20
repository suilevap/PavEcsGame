using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PavEcsGame.Common.Utils;

namespace PavEcsGame.Tiles
{
    public class WallRuleProvider
    {
        private readonly string[] _availableRules;

        public WallRuleProvider()
        {
            _availableRules = DiscoverAvailableWallRules();
        }

        public string[] AvailableRules => _availableRules;

        public string SelectRandomRule(Random random)
        {
            return _availableRules[random.Next(_availableRules.Length)];
        }

        private string[] DiscoverAvailableWallRules()
        {
            var rules = new List<string>();

            // Add base wall rule if it exists
            if (File.Exists(FileHelper.ResolvePath("Data/wall_rule.txt")))
                rules.Add("wall_rule");

            // Scan wall_styles folder
            var stylesDir = FileHelper.ResolvePath("Data/wall_styles");
            if (Directory.Exists(stylesDir))
            {
                var styleFiles = Directory.GetFiles(stylesDir, "*.txt").OrderBy(f => f);
                foreach (var file in styleFiles)
                {
                    var filename = Path.GetFileNameWithoutExtension(file);
                    rules.Add($"wall_styles/{filename}");
                }
            }

            return rules.Count > 0 ? rules.ToArray() : new[] { "wall_rule" };
        }
    }
}
