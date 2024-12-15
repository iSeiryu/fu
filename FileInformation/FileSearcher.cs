using System.Runtime.InteropServices;
using Spectre.Console;

namespace FileInformation.Cli;

public static class FileSearcher {
    public static IEnumerable<FileInfo> Search(string searchPath, string searchPattern, bool includeHidden, bool recurseSubdirectories) {
        var searchOptions = CreateEnumerationOptions(includeHidden);
        if (recurseSubdirectories) {
            var files = new List<FileInfo>();
            Search(searchPath, searchPattern, searchOptions, files);
            return files;
        }
        
        return new DirectoryInfo(searchPath).EnumerateFiles(searchPattern, searchOptions);
    }

    static void Search(string searchPath,
        string searchPattern,
        EnumerationOptions searchOptions,
        List<FileInfo> files) {
        files.AddRange(new DirectoryInfo(searchPath).GetFiles(searchPattern, searchOptions));

        var dirs = Directory.GetDirectories(searchPath, "*", searchOptions);

        if (dirs.Length == 0) return;
        if (dirs.Length == 1) {
            Search(dirs[0], searchPattern, searchOptions, files);
            return;
        }

        //Parallel.ForEach(dirs, dir => {
        foreach (var dir in dirs) {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && dir == "/proc")
                continue;

            Search(dir, searchPattern, searchOptions, files);
        }
        //});
    }

    static EnumerationOptions CreateEnumerationOptions(bool includeHidden) {
        var searchOptions = new EnumerationOptions {
            AttributesToSkip =
                FileAttributes.ReparsePoint,
            RecurseSubdirectories = false
        };
        
        if (!includeHidden) {
            searchOptions.AttributesToSkip |= FileAttributes.Hidden;
        }
        
        return searchOptions;
    }
}