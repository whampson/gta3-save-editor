namespace GTA3SaveEditor.Core.Extensions
{
    public static class PathEx
    {
        public static string AppendTrailingSlash(string path)
        {
            if (path != null && !path.EndsWith("\\"))
            {
                path += "\\";
            }

            return path;
        }
    }
}
