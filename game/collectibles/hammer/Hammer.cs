using Godot;
using System;
using System.Threading.Tasks;
using Theadventureofhink.autoloads;
using Theadventureofhink.effects.achievement_effect;
using Theadventureofhink.game_state;

public partial class Hammer : Node2D
{
    private AchievementEffect _achievementEffect;
    private CutsceneManager _cutsceneManager;
    private GameState _gameState;

    public override void _Ready()
    {
        _achievementEffect = GetNode<AchievementEffect>("CanvasLayer/AchievementEffect");
        _cutsceneManager = GetNode<CutsceneManager>(Singletons.CutsceneManager);
        _gameState = GetNode<GameState>(Singletons.GameState);
    }

    public async Task CollectHammer()
    {
        GD.Print("Collect hammer");
        GD.Print("_achievementEffect", _achievementEffect);
        _achievementEffect.Start();

        await _cutsceneManager.PlaySingleCharacterLine("Hink", "Oh, nice smashy-hammer!");

        _achievementEffect.Stop();

        _gameState.PlayerState.PlayerItemsState.GotBlacksmithsHammers.SetValue(true);
    }
}