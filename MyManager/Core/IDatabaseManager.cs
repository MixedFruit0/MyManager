namespace MyManager.Core;

public interface IDatabaseManager
{
    Task<List<string>> GetTableNamesAsync();
}