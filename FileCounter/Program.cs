using FileCounter.Cli;
using Spectre.Console.Cli;

var app = new CommandApp();
app.Configure(config => {
    config.AddCommand<CountAllFiles>("count");
    config.AddCommand<GetSizeOfAllFiles>("size");
});

return app.Run(args);
