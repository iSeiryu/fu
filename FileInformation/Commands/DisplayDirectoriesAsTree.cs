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

        [CommandOption("-d|--depth")]
        [DefaultValue(3)]
        public int Depth { get; init; }

        [CommandOption("--hidden")]
        [DefaultValue(false)]
        public bool IncludeHidden { get; init; }
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings) {
        AnsiConsole
            .Status()
            .Spinner(FuSettings.Spinner)
            .SpinnerStyle(Style.Parse("green"))
            .Start("Working...", ctx => {
                AnsiConsole.MarkupLine($"Searching files in [green]{settings.SearchPath}[/]");
                Search(settings);
            });

        return 0;
    }

    static void Search(Settings settings) {
        var searchPath = PathService.BuildPath(settings.SearchPath);

        var searchOptions = new EnumerationOptions {
            AttributesToSkip = settings.IncludeHidden
                ? FileAttributes.System
                : FileAttributes.Hidden | FileAttributes.System
        };

        var root = new Tree(searchPath).Style("red");
        SearchRecursively(new DirectoryInfo(searchPath), root, settings.Depth);

        AnsiConsole.Write(root);

        void SearchRecursively(DirectoryInfo directory, IHasTreeNodes tree, int depth) {
            if (depth <= 0) {
                return;
            }

            var subDirectories = directory.EnumerateDirectories("*", searchOptions);
            foreach (var subDirectory in subDirectories) {
                var node = tree.AddNode(subDirectory.Name);
                SearchRecursively(subDirectory, node, depth - 1);
            }

            var subFiles = directory.EnumerateFiles("*", searchOptions);
            foreach (var subFile in subFiles) {
                var curr = subFile.Name;

                if (subFile.Name.Contains('['))
                    curr = subFile.Name.EscapeMarkup();

                tree.AddNode($"[blue]{curr}[/]");
            }
        }
    }
}
