using FileInformation.Cli.Commands;
using Spectre.Console.Cli;

var app = new CommandApp();
app.Configure(config => {
    config.AddCommand<CountAllFiles>("count");
    config.AddCommand<GetSizeOfAllFiles>("size");
    config.AddCommand<DisplayDirectoriesAsTree>("tree");
    config.AddCommand<Fu>("fu").IsHidden();
});

return app.Run(args);
