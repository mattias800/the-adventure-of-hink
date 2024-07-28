using Godot;
using Theadventureofhink.autoloads;
using Theadventureofhink.effects.achievement_effect;

namespace Theadventureofhink.collectibles.walljump;

public partial class WalljumpChest : Node2D
{
    private CanvasLayer _canvasLayer;
    private AchievementEffect _achievementEffect;

    private CutsceneManager _cutsceneManager;

    public override void _Ready()
    {
        _cutsceneManager = GetNode<CutsceneManager>(Singletons.CutsceneManager);
        _canvasLayer = GetNode<CanvasLayer>("CanvasLayer");
        _achievementEffect = GetNode<AchievementEffect>("CanvasLayer/AchievementEffect");
    }

    public override void _Process(double delta)
    {
    }

    public async void OnOpen()
    {
        var dialog = GD.Load("res://collectibles/walljump/walljump_dialogue.dialogue");

        _achievementEffect.Start();
        GD.Print("OnOpen cheeesty");

        await _cutsceneManager.StartTimeline(dialog, "start", 1);
        
        GD.Print("OnOpen cheeesty DONE");
        _achievementEffect.Stop();
    }
}