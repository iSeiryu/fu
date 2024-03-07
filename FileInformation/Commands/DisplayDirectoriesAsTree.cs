using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Spectre.Console;
using Spectre.Console.Cli;

namespace FileInformation.Cli.Commands;

internal sealed class DisplayDirectoriesAsTree : Command<DisplayDirectoriesAsTree.Settings> {
    public sealed class Settings : CommandSettings {
        [Description("Path to search. Defaults to current directory.")]
        [CommandArgument(0, "[searchPath]")]
        public string? SearchPath { get; init; }
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings) {
        AnsiConsole
            .Status()
            .Spinner(Spinner.Known.Clock)
            .SpinnerStyle(Style.Parse("green"))
            .Start("Working...", ctx => {
                AnsiConsole.MarkupLine($"Searching files in [green]{settings.SearchPath}[/]");
                Search(settings);
            });

        return 1;
    }

    static void Search(Settings settings) {
        var searchPath = settings.SearchPath ?? Directory.GetCurrentDirectory();
        if (searchPath.StartsWith("~/") || searchPath.StartsWith("~\\")) {
            var homeFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + Path.DirectorySeparatorChar;
            searchPath = searchPath.Replace("~/", homeFolder);
            searchPath = searchPath.Replace("~\\", homeFolder);
        }

        var searchOptions = new EnumerationOptions {
            AttributesToSkip = FileAttributes.System,
        };

        var root = new Tree(searchPath).Style("red");
        var directories = new DirectoryInfo(searchPath).EnumerateDirectories("*", searchOptions);
        foreach (var directory in directories) {
            root.AddNode(directory.Name);
        }

        var files = new DirectoryInfo(searchPath).EnumerateFiles("*", searchOptions);
        foreach (var file in files) {
            root.AddNode($"[blue]{file.Name}[/]");
        }

        AnsiConsole.Write(root);
    }
}
