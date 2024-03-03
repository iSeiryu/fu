using FileCounter;
using Spectre.Console.Cli;

var app = new CommandApp();
app.Configure(config => {
    config.AddCommand<CountAllFiles>("count");
});

return app.Run(args);
