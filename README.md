# MyManager

MyManager is a console-based MySQL manager built with C# and [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0). It uses [Spectre.Console](https://github.com/spectreconsole/spectre.console) for rich terminal UI and provides a simple interface for interacting with a MySQL database.

![Screenshot](https://i.imgur.com/NxuPJXr.png)

## Features

- View all tables in a connected database
- Run raw SQL queries
- Insert, update, and delete rows interactively
- Edit and save connection settings
- Fully terminal-based, clean interface with Spectre.Console

## Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/MixedFruit0/MyManager.git
   ```

2. Open the project in Visual Studio or any .NET-compatible IDE.  
   Or run directly from terminal:
   ```bash
   dotnet run --project MyManager
   ```

3. If no connection settings are found, you'll be prompted to enter them.

## Requirements

- .NET 8 SDK
- A running MySQL database
- Connection credentials (host, port, username, password, database)

## Notes

- `Data/config.json` is ignored via `.gitignore` since it stores your database credentials.
- You’ll be prompted to create it when launching the app for the first time if it’s missing.

## License

This project is licensed under the MIT License.
