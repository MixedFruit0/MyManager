using Spectre.Console;
using MyManager.Core;
using MyManager.Models;
using MyManager.Services;
using MySqlConnector;
using static MyManager.Utils.ConsoleHelpers;

namespace MyManager.UI;

public static class Menu
{
    public static async Task ShowMainAsync()
    {
        while (true)
        {
            Console.Clear();
            Write(new FigletText("MyManager").Centered().Color(Color.Blue));

            var choice = Prompt(new SelectionPrompt<string>()
                .Title("[green]Select an action:[/]")
                .AddChoices(new[]
                {
                    "View tables",
                    "Run Sql Query",
                    "Insert row",
                    "Update row",
                    "Delete row",
                    "Connection settings",
                    "Exit"
                }));

            switch (choice)
            {
                case "View tables":
                    await ViewTablesAsync();
                    break;
                
                case "Run Sql Query":
                    await RunSqlQueryAsync();
                    break;
                
                case "Insert row":
                    await InsertRowAsync();
                    break;
                
                case "Update row":
                    await UpdateRowAsync();
                    break;
                
                case "Delete row":
                    await DeleteRowAsync();
                    break;
                
                case "Connection settings":
                    await ConnectionSettingsAsync();
                    break;
                
                case "Exit":
                    Console.Clear();
                    Ok("Exiting MyManager...");
                    Thread.Sleep(500);
                    Environment.Exit(0);
                    return;
            }
        }
    }
    
    private static async Task RunSqlQueryAsync()
    {
        Console.Clear();

        var connectionString = SettingsService.Current?.GetConnectionString();
        if (string.IsNullOrEmpty(connectionString))
        {
            Error("Missing connection settings");
            Wait();
            return;
        }

        var sql = Ask("SQL>", "");

        try
        {
            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var cmd = new MySqlCommand(sql, connection);
            var reader = await cmd.ExecuteReaderAsync();

            Log(""); // spacing

            while (await reader.ReadAsync())
            {
                string row = "";
                for (int i = 0; i < reader.FieldCount; i++)
                    row += $"{reader[i]} ";

                MarkupLine(row.TrimEnd());
            }
        }
        catch (Exception ex)
        {
            Error(ex.ToString());
        }

        Wait();
    }
    
    private static async Task ViewTablesAsync()
    {
        Console.Clear();
        
        var connectionString = SettingsService.Current?.GetConnectionString();
        if (string.IsNullOrEmpty(connectionString))
        {
            Error("Missing connection settings");
            Wait();
            return;
        }

        try
        {
            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var tables = new List<string>();
            var cmd = new MySqlCommand("SHOW TABLES;", connection);
            using var reader = await cmd.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
                tables.Add(reader.GetString(0));

            if (tables.Count == 0)
            { 
                Warn("No tables found");
            }
            else
            {
                var table = new Table().RoundedBorder().BorderColor(Color.Blue);
                table.AddColumn("[green]Tables[/]");

                foreach (var name in tables)
                    table.AddRow(name);
                
                Write(table);
            }
        }
        catch (Exception ex)
        {
            Error(ex.ToString());
        }
        Wait();
    }

    private static async Task InsertRowAsync()
    {
        Console.Clear();

        var connectionString = SettingsService.Current?.GetConnectionString();
        if (string.IsNullOrEmpty(connectionString))
        {
            Error("Missing connection settings");
            Wait();
            return;
        }

        var manager = new MySqlManager(connectionString);
        var tables = await manager.GetTableNamesAsync();

        if (tables.Count == 0)
        {
            Warn("No tables found");
            Wait();
            return;
        }

        // pick the table to insert into
        var table = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("Pick a table:")
            .PageSize(10)
            .AddChoices(tables));

        // get how many fields user wants to insert
        var count = Ask<int>("How many fields?", 1);

        if (count <= 0)
        {
            Warn("Zero fields, nothing to insert");
            Wait();
            return;
        }

        var fields = new List<(string column, string value)>();

        for (int i = 0; i < count; i++)
        {
            var col = Ask<string>($"Field #{i +1} name:", "");
            var val = Ask<string>($"Value for {col}:", "");
            fields.Add((col, val));
        }

        var columns = string.Join(", ", fields.Select(f => $"`{f.column}`"));
        var values = string.Join(", ", fields.Select(f => $"'{f.value.Replace("'", "''")}'"));

        var sql = $"INSERT INTO `{table}` ({columns}) VALUES ({values})";

        try
        {
            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var cmd = new MySqlCommand(sql, connection);
            var rows = await cmd.ExecuteNonQueryAsync();

            Ok($"{rows} row inserted into {table}!");
        }
        catch (Exception ex)
        {
            Error(ex.ToString());
        }
        
        Wait();
    }

    private static async Task UpdateRowAsync()
    {
        Console.Clear();

        var connectionString = SettingsService.Current?.GetConnectionString();
        if (string.IsNullOrEmpty(connectionString))
        {
            Error("Missing connection settings");
            Wait();
            return;
        }

        var manager = new MySqlManager(connectionString);
        var tables = await manager.GetTableNamesAsync();

        if (tables.Count == 0)
        {
            Log("No tables found");
            Wait();
            return;
        }

        var table = Prompt(new SelectionPrompt<string>()
            .Title("Update which table?")
            .PageSize(10)
            .AddChoices(tables));

        var where = Ask<string>("WHERE condition (example `id = 10`):", "");
        var count = Ask<int>("How many fields?", 0);

        if (count <= 0)
        {
            Log("Zero fields, nothing to do");
            Wait();
            return;
        }
        
        var fields = new List<(string column, string value)>();

        for(int i = 0; i < count; i++)
        {
            var col = Ask<string>($"Field #{i + 1} name:", "");
            var val = Ask<string>($"New value for {col}:", "");
            fields.Add((col, val));
        }
        
        var set = string.Join(", ", fields.Select(f => $"`{f.column}` = '{f.value.Replace("'", "''")}'"));
        var sql = $"UPDATE `{table}` SET {set} WHERE {where}";

        try
        {
            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var cmd = new MySqlCommand(sql, connection);
            var rows = await cmd.ExecuteNonQueryAsync();
                        
            Log($"{rows} rows updated in {table}");
        }
        catch (Exception ex)
        {
            Error(ex.ToString());
        }
        
        Wait();
    }

    private static async Task DeleteRowAsync()
    {
        Console.Clear();

        var connectionString = SettingsService.Current?.GetConnectionString();
        if (string.IsNullOrEmpty(connectionString))
        {
            Error("Missing connection settings");
            Wait();
            return;
        }

        var manager = new MySqlManager(connectionString);
        var tables = await manager.GetTableNamesAsync();

        if (tables.Count == 0)
        {
            Warn("No tables found");
            Wait();
            return;
        }

        var table = Prompt(new SelectionPrompt<string>()
            .Title("Delete from which table?")
            .PageSize(10)
            .AddChoices(tables));

        var where = Ask<string>("WHERE condition (example `id = 5`)", "");

        if (string.IsNullOrWhiteSpace(where))
        {
            Warn("No WHERE condition entered, cancelled");
            Wait();
            return;
        }

        bool confirm = Confirm("Are you sure you want to delete matching rows?");
        if (!confirm)
        {
            Warn("Deletion cancelled");
            Wait();
            return;
        }

        var sql = $"DELETE FROM `{table}` WHERE {where}";

        try
        {
            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var cmd = new MySqlCommand(sql, connection);
            var rows = await cmd.ExecuteNonQueryAsync();

            if (rows == 0)
                Warn("No rows matched the condition");
            else
                Ok($"{rows} rows deleted from {table}!");
        }
        catch (Exception ex)
        {
            Error(ex.ToString());
        }

        Wait();
    }

    private static async Task ConnectionSettingsAsync()
    {
        Console.Clear();
        
        WriteLine("Edit connection settings:");
        
        var host = Ask("Host", "localhost");
        var port = Ask("Port", 3306);
        var user = Ask("Username", "root");
        var password = Prompt(new TextPrompt<string>("Password").PromptStyle("red").Secret());
        var db = Ask("Db", "test");

        SettingsService.Current = new ConnectionSettings()
        {
            Host = host,
            Port = port,
            Username = user,
            Password = password,
            Database = db
        };
        
        Ok("Saved connection settings");
        Wait();
    }
}