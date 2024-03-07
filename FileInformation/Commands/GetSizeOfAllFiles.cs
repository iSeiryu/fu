using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Spectre.Console;
using Spectre.Console.Cli;

namespace FileInformation.Cli.Commands;
internal sealed class GetSizeOfAllFiles : Command<GetSizeOfAllFiles.Settings> {
    public sealed class Settings : CommandSettings {
        [Description("Path to search. Defaults to current directory.")]
        [CommandArgument(0, "[searchPath]")]
        public string? SearchPath { get; init; }

        [CommandOption("-p|--pattern")]
        public string? SearchPattern { get; init; }

        [CommandOption("--head")]
        [DefaultValue(0)]
        public int Head { get; init; }

        [CommandOption("--tail")]
        [DefaultValue(0)]
        public int Tail { get; init; }

        [CommandOption("-r|--recurse")]
        [DefaultValue(false)]
        public bool RecurseSubdirectories { get; init; }

        [CommandOption("--hidden")]
        [DefaultValue(false)]
        public bool IncludeHidden { get; init; }
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings) {
        AnsiConsole
            .Status()
            .Spinner(Spinner.Known.Clock)
            .SpinnerStyle(Style.Parse("green"))
            .Start("Working...", ctx => {
                AnsiConsole.MarkupLine($"Searching files in [green]{settings.SearchPath}[/]");
                var (searchPattern, searchPath) = SanitizeInput(settings);
                var files = Search(searchPath, searchPattern, settings);
                var result = Aggregate(settings, files);

                PrintResults(settings, result, searchPath, searchPattern);
            });

        return 1;
    }

    static IEnumerable<FileInfo> Search(string searchPath, string searchPattern, Settings settings) {
        var searchOptions = new EnumerationOptions {
            AttributesToSkip = settings.IncludeHidden
                ? FileAttributes.System
                : FileAttributes.Hidden | FileAttributes.System,
            RecurseSubdirectories = settings.RecurseSubdirectories
        };

        return new DirectoryInfo(searchPath).EnumerateFiles(searchPattern, searchOptions);
    }

    static List<KeyValuePair<string, long>> Aggregate(Settings settings, IEnumerable<FileInfo> files) {
        var groupped = files.Aggregate(
                    new Dictionary<string, long>(),
                                (acc, fileInfo) => {
                                    var path = fileInfo.DirectoryName;
                                    if (acc.ContainsKey(path!)) {
                                        acc[path!] += fileInfo.Length;
                                    }
                                    else {
                                        acc[path!] = fileInfo.Length;
                                    }

                                    return acc;
                                })
                            .OrderByDescending(x => x.Value);

        var result = groupped.ToList();
        if (settings.Head > 0 || settings.Tail > 0) {
            result = [];

            if (settings.Head > 0)
                result.AddRange(groupped.Take(settings.Head));
            if (settings.Tail > 0)
                result.AddRange(groupped.TakeLast(settings.Tail));
        }

        return result;
    }

    static void PrintResults(Settings settings, IEnumerable<KeyValuePair<string, long>> result, string searchPath, string searchPattern) {
        var includingHidden = settings.IncludeHidden ? ", including hidden" : "";
        var includingSubdirectories = settings.RecurseSubdirectories ? ", including subdirectories" : "";
        var totalFileSize = result.Sum(x => x.Value);

        foreach (var (key, value) in result) {
            AnsiConsole.MarkupLine($"[blue]{value:N0}[/]\t[green]{key}[/]");
        }

        AnsiConsole.MarkupLine($"Total file size for [green]{searchPattern}[/] files in [green]{searchPath}[/]{includingHidden}{includingSubdirectories}");
        AnsiConsole.MarkupLine($"[blue]{totalFileSize:N0}[/] bytes");
        AnsiConsole.MarkupLine($"[blue]{totalFileSize / 1000m:F2}[/] KB");
        AnsiConsole.MarkupLine($"[blue]{totalFileSize / 1000m / 1000:F4}[/] MB");
    }

    static (string searchPattern, string searchPath) SanitizeInput(Settings settings) {
        var searchPattern = settings.SearchPattern ?? "*";
        var searchPath = PathService.BuildPath(settings.SearchPath);

        return (searchPattern, searchPath);
    }
}