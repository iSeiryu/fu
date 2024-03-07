using FileInformation.Cli;
using Spectre.Console.Cli;

var app = new CommandApp();
app.Configure(config => {
    config.AddCommand<CountAllFiles>("count");
    config.AddCommand<GetSizeOfAllFiles>("size");
    config.AddCommand<DisplayDirectoriesAsTree>("tree");
});

return app.Run(args);
