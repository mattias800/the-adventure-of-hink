#nullable enable
using System.Text.Json;
using Godot;
using Theadventureofhink.game_state;

namespace Theadventureofhink.game_saves;

public static class GameSaveFileReader
{
    private static JsonSerializerOptions options = new JsonSerializerOptions
    {
        IncludeFields = true,
        WriteIndented = true // This option is for pretty-printing, optional
    };

    public static void SaveGameStateToFile(GameState gameState, string filename)
    {
        using var saveFile = FileAccess.Open($"user://{filename}.save", FileAccess.ModeFlags.Write);
        var jsonString = JsonSerializer.Serialize(gameState, options);
        GD.Print("Save game");
        GD.Print(jsonString);
        saveFile.StoreString(jsonString);
    }

    public static GameState? LoadFileToGameState(string filename)
    {
        using var saveFile = FileAccess.Open($"user://{filename}.save", FileAccess.ModeFlags.Read);
        if (saveFile != null)
        {
            var jsonString = saveFile.GetAsText();
            GD.Print("Load game");
            GD.Print(jsonString);
            return JsonSerializer.Deserialize<GameState>(jsonString, options);
        }

        return null;
    }
}