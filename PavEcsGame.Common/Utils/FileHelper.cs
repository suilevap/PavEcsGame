using System.IO;
using System.Reflection;

namespace PavEcsGame.Common.Utils
{
    public static class FileHelper
    {
        public static string ResolvePath(string path)
        {
            if (Path.IsPathRooted(path))
                return path;
            if (File.Exists(path) || Directory.Exists(path))
                return path;

            // Fall back to entry point assembly directory for dotnet run compatibility
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null && !string.IsNullOrEmpty(entryAssembly.Location))
            {
                var baseDir = Path.GetDirectoryName(entryAssembly.Location);
                if (!string.IsNullOrEmpty(baseDir))
                {
                    var fallbackPath = Path.Combine(baseDir, path);
                    if (File.Exists(fallbackPath) || Directory.Exists(fallbackPath))
                        return fallbackPath;
                }
            }

            return path; // Return original path if nothing resolves
        }
    }
}
