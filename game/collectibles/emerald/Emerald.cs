using Godot;
using System;
using Theadventureofhink.autoloads;
using Theadventureofhink.game_state;

public partial class Emerald : Node2D
{
    [Signal]
    public delegate void EmeraldCollectedEventHandler();

    [Export] public CollectibleInstance Instance;
    [Export] public bool ShowDialogue = true;

    private CutsceneManager _cutsceneManager;

    public override void _Ready()
    {
        _cutsceneManager = GetNode<CutsceneManager>(Singletons.CutsceneManager);
        GetNode<Theadventureofhink.collectibles.collectible.Collectible>("Collectible").Instance = Instance;
    }

    public async void OnCollected()
    {
        if (ShowDialogue)
        {
            await _cutsceneManager.PlaySingleCharacterLine("Hink", "Ooh, shiny!");
        }

        EmitSignal(SignalName.EmeraldCollected);
    }
}