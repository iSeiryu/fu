namespace FileInformation.Cli;

internal static class PathHelper {
    public static string BuildPath(string? path) {
        var searchPath = path ?? Directory.GetCurrentDirectory();
        if (searchPath.StartsWith("~/") || searchPath.StartsWith("~\\")) {
            var homeFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + Path.DirectorySeparatorChar;
            searchPath = searchPath.Replace("~/", homeFolder);
            searchPath = searchPath.Replace("~\\", homeFolder);
        }

        return searchPath;
    }
}
