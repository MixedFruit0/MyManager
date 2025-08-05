using MyManager.UI;

namespace  MyManager;

internal static class Program
{
    static async Task Main(string[] args)
    {
        new App().Initialize();
        await Menu.ShowMainAsync();
    }
}

