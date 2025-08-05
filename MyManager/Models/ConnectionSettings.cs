namespace MyManager.Models;

public class ConnectionSettings
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 3306;
    public string Username { get; set; } = "root";
    public string Password { get; set; } = string.Empty;
    public string Database { get; set; } = "test";

    public string GetConnectionString()
    {
        return $"Server={Host};Port={Port};User ID={Username};Password={Password};Database={Database};";
    }
}
