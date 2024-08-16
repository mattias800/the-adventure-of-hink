using System.Text.Json;
using Godot;
using Theadventureofhink.game_state;

namespace Theadventureofhink.game_saves;

public static class GameSaveFileReader
{
    public static void SaveGameStateToFile(GameState gameState, string filename)
    {
        using var saveFile = FileAccess.Open($"user://{filename}.save", FileAccess.ModeFlags.Write);
        var jsonString = JsonSerializer.Serialize(gameState);
        saveFile.StoreLine(jsonString);
    }

    public static GameState LoadFileToGameState(string filename)
    {
        using var saveFile = FileAccess.Open($"user://{filename}.save", FileAccess.ModeFlags.Read);
        var jsonString = saveFile.GetLine();
        GD.Print("Load game");
        GD.Print(jsonString);
        return JsonSerializer.Deserialize<GameState>(jsonString);
    }
}