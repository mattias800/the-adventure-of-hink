using Godot;
using Theadventureofhink.autoloads;
using Theadventureofhink.effects.achievement_effect;
using Theadventureofhink.entities.chest;
using Theadventureofhink.game_state;

namespace Theadventureofhink.collectibles.walljump;

public partial class WalljumpChest : Node2D
{
    private CanvasLayer _canvasLayer;
    private AchievementEffect _achievementEffect;

    private Chest _chest;
    private GameState _gameState;
    private CutsceneManager _cutsceneManager;

    public override void _Ready()
    {
        _cutsceneManager = GetNode<CutsceneManager>(Singletons.CutsceneManager);
        _gameState = GetNode<GameState>(Singletons.GameState);
        _canvasLayer = GetNode<CanvasLayer>("CanvasLayer");
        _chest = GetNode<Chest>("Chest");
        _achievementEffect = GetNode<AchievementEffect>("CanvasLayer/AchievementEffect");

        if (_gameState.PlayerState.PlayerSkillsState.CanClimbWalls.Value())
        {
            _chest.IsOpen = true;
        }
    }
    
    public async void OnOpen()
    {
        var dialog = GD.Load("res://collectibles/walljump/walljump_dialogue.dialogue");

        _achievementEffect.Start();

        await _cutsceneManager.StartTimeline(dialog, "start", 1);
        
        _achievementEffect.Stop();

        _gameState.PlayerState.PlayerSkillsState.CanClimbWalls.SetValue(true);
        _gameState.PlayerState.PlayerSkillsState.CanWallJump.SetValue(true);
    }
}