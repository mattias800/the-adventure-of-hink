using System.Threading.Tasks;
using Godot;
using Theadventureofhink.autoloads;
using Theadventureofhink.game_state;

namespace Theadventureofhink.world.characters.little_mushroom;

public partial class LittleMushroom : Node2D
{
    private AnimatedSprite2D _animatedSprite2D;
    private Talkable _talkable;

    private Resource _resource;

    private GameManager _gameManager;
    private GameStateManager _gameStateManager;
    private CutsceneManager _cutsceneManager;

    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>(Singletons.GameManager);
        _gameStateManager = GetNode<GameStateManager>(Singletons.GameStateManager);
        _cutsceneManager = GetNode<CutsceneManager>(Singletons.CutsceneManager);

        _animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _talkable = GetNode<Talkable>("Talkable");
        _resource = GD.Load("res://world/characters/little_mushroom/little_mushroom.dialogue");

        _animatedSprite2D.Play("idle");
    }

    public override void _Process(double delta)
    {
        if (_gameManager.Player.GlobalPosition < GlobalPosition)
        {
            _animatedSprite2D.FlipH = true;
        }
        else
        {
            _animatedSprite2D.FlipH = false;
        }
    }

    public async Task Mushy(string s)
    {
        await _cutsceneManager.PlayDialogueCharacterLine("Mushy", s);
    }

    public async Task Hink(string s)
    {
        await _cutsceneManager.PlayDialogueCharacterLine("Hink", s);
    }

    public async void OnTalk()
    {
        _cutsceneManager.StartDialogue();

        if (_gameStateManager.GameState.PlayerState.PlayerSkillsState.CanDoubleJump.Value)
        {
            await Mushy("Wow, you can double jump?!");
            await Mushy("That might help against those [shake rate=20 level=5]SPIKES[/shake]!");
        }
        else if (!_gameStateManager.GameState.CharactersState.LittleMushroomState.HasEverMet.Value)
        {
            _gameStateManager.GameState.CharactersState.LittleMushroomState.HasEverMet.SetValue(true);
            await Mushy("Ahh, I have you seen the [shake rate=20 level=5]SPIKES[/shake]?!");
            await Hink("Yeah, I noticed.");
            await Hink("Are they yours?");
            await Mushy("Oh, no, I do [shake rate=20 level=5]NOT[/shake] like spikes.");
            await Hink("Well ok then, take care!");
            await Mushy("I [shake rate=20 level=5]HATE[/shake] spikes.");
        }
        else
        {
            await Mushy("Holy shit, [shake rate=20 level=5]SPIKES[/shake]!");
        }

        _cutsceneManager.EndDialogue();

        _talkable.Activate();
    }
}