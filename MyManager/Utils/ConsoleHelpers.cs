using Spectre.Console;
using Spectre.Console.Rendering;

namespace MyManager.Utils;

public static class ConsoleHelpers
{
    public static void Log(string text) => AnsiConsole.MarkupLine($"[blue]{text}[/]");
    public static void Ok(string text) => AnsiConsole.MarkupLine($"[green]{text}[/]");
    public static void Warn(string text) => AnsiConsole.MarkupLine($"[yellow]{text}[/]");
    public static void Error(string text) => AnsiConsole.MarkupLine($"[red]Error:[/] {text}");
    public static void MarkupLine(string text) => AnsiConsole.MarkupLine(text);
    public static void WriteLine(string? text) => AnsiConsole.WriteLine(text ?? string.Empty);
    public static void Write(IRenderable renderable) => AnsiConsole.Write(renderable);
    public static bool Confirm(string message) => AnsiConsole.Confirm($"[yellow]{message}[/]");
    public static T Prompt<T>(IPrompt<T> prompt) => AnsiConsole.Prompt(prompt);

    public static T Ask<T>(string prompt, T defaultValue)
    {
        string input;

        if (defaultValue == null || defaultValue?.ToString() == "")
            input = AnsiConsole.Ask<string>(prompt);
        else
        {
            input = AnsiConsole.Ask<string>($"{prompt} (default: {defaultValue})");
        }

        if (string.IsNullOrEmpty(input))
            return defaultValue;

        try
        {
            return (T)Convert.ChangeType(input, typeof(T));
        }
        catch
        {
            return defaultValue;
        }
    }

    public static void Wait(string? message = null)
    {
        if (!string.IsNullOrEmpty(message))
            Log(message);

        Console.ReadKey(true);
    }
}