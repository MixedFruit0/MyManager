using MyManager.Models;
using MyManager.Services;
using Spectre.Console;
using static MyManager.Utils.ConsoleHelpers;

namespace MyManager;

public sealed class App
{
    public void Initialize()
    {
        SettingsService.Load();
        
        if (SettingsService.Current is null)
            SetupConnection();
    }

    private void SetupConnection()
    {
        Console.Clear();
        Log("No config file found, enter your db info:\n");

        var host = Ask("Host:", "localhost");
        var port = Ask("Port:", 3306);
        var user = Ask("User:", "root");
        var password = Prompt(new TextPrompt<string>("Password:")
            .PromptStyle("red").Secret());
        var db = Ask("Db:", "test");

        var settings = new ConnectionSettings()
        {
            Host = host,
            Port = port,
            Username = user,
            Password = password,
            Database = db
        };

        SettingsService.Save(settings);

        Log("\nConfig saved. Press any key");
        Wait();
    }
}
