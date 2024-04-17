using FileInformation.Cli.Commands;
using FileInformation.Cli.Commands.Count;
using Spectre.Console.Cli;

var app = new CommandApp();
app.Configure(config => {
    config.AddBranch<CommandSettings>("count", count => {
        count.AddCommand<CountAllFiles>("files")
             .WithDescription("Count files in a directory and group them by extension.");
        count.AddCommand<CountWords>("words")
             .WithDescription("Count words in files in a directory.");
    });

    config.AddCommand<GetSizeOfAllFiles>("size").WithDescription("Count the size of all files in a directory.");
    config.AddCommand<DisplayDirectoriesAsTree>("tree").WithDescription("Display files and directories as a tree.");
    config.AddCommand<Fu>("fu").IsHidden();
});

return app.Run(args);
