using FileInformation.Cli.Commands;
using FileInformation.Cli.Commands.Count;
using Spectre.Console.Cli;

var app = new CommandApp();
app.Configure(config => {
    config.AddBranch<CommandSettings>("count", count => {
        count.SetDescription("Count files, words, code.");

        count.AddCommand<CountAllFiles>("files")
             .WithDescription("Count files in a directory and group them by extension.");
        count.AddCommand<CountWords>("words")
             .WithDescription("Count words in files in a directory.")
             .WithExample(["words", ".", "'foo'", "'bar'", "-r"])
             .WithExample(["words", "-r", "--hidden"]);
    });

    config.AddCommand<GetSizeOfAllFiles>("size").WithDescription("Calculate the size of all files in a directory.");
    config.AddCommand<DisplayDirectoriesAsTree>("tree").WithDescription("Display files and directories as a tree.");
    config.AddCommand<Fu>("fu").IsHidden();

    config.AddExample(["size", "~/", "--hidden", "-r"]);
    config.Settings.ApplicationName = "fu";
});

//return app.Run(["size", "/", "--hidden", "-r", "-p", "*.png"]);
return app.Run(args);
