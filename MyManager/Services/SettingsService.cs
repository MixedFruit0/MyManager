using System.Text.Json;
using MyManager.Models;

namespace MyManager.Services;

public static class SettingsService
{
    private const string SettingsPath = "Data/config.json";
    public static ConnectionSettings? Current { get; set; }

    public static void Load()
    {
        if (!File.Exists(SettingsPath))
        {
            Current = null;
            return;
        }

        var json = File.ReadAllText(SettingsPath);
        Current = JsonSerializer.Deserialize<ConnectionSettings>(json);
    }

    public static void Save(ConnectionSettings settings)
    {
        Directory.CreateDirectory("Data");
        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(SettingsPath, json);
        Current = settings;
    }
}

