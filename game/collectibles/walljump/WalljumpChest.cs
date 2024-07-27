using Godot;
using System;

public partial class WalljumpChest : Node2D
{
    private CanvasLayer _canvasLayer;
    private AchievementEffect _achievementEffect;

    public override void _Ready()
    {
        _canvasLayer = GetNode<CanvasLayer>("CanvasLayer");
        _achievementEffect = GetNode<AchievementEffect>("CanvasLayer/AchievementEffect");
    }

    public override void _Process(double delta)
    {
    }

    public async void OnOpen()
    {
        var cutsceneManager = GetNode("/root/CutsceneManager");
        
        var dialog = GD.Load("res://collectibles/walljump/walljump_dialogue.dialogue");

        _achievementEffect.Start();
        GD.Print("OnOpen cheeesty");

        cutsceneManager.Call("start_timeline", dialog, "start", 1);

        await ToSignal(cutsceneManager, "cutscene_ended");
        GD.Print("Done!");
        _achievementEffect.Stop();
    }
}