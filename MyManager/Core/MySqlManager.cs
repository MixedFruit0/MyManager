using MySqlConnector;

namespace MyManager.Core;

public class MySqlManager : IDatabaseManager
{
    private readonly string _connectionString;

    public MySqlManager(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<List<string>> GetTableNamesAsync()
    {
        var tables = new List<string>();
        
        // open connection and fetch table names
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        using var cmd = new MySqlCommand("SHOW TABLES;", connection);
        using var reader = await cmd.ExecuteReaderAsync();

        while(await reader.ReadAsync())
        {
            tables.Add(reader.GetString(0));
        }
        return tables;
    }
}
   