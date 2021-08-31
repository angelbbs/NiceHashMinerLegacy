using System.IO;

namespace MyDownloader.Core.Common
{
    public static class PathHelper
    {
        public static string GetWithBackslash(string path)
        {
            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                path += Path.DirectorySeparatorChar.ToString();
            }

            return path;
        }
    }
}
