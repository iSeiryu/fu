using FileInformation.Cli.Commands;
using Spectre.Console.Cli;

var app = new CommandApp();
app.Configure(config => {
    config.AddCommand<CountAllFiles>("count").WithDescription("Count files in a directory and group them by extension.");
    config.AddCommand<GetSizeOfAllFiles>("size").WithDescription("Count the size of all files in a directory.");
    config.AddCommand<DisplayDirectoriesAsTree>("tree").WithDescription("Display files and directories as a tree.");
    config.AddCommand<Fu>("fu").IsHidden();
});

return app.Run(args);
