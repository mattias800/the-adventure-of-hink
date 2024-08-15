using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;
using Theadventureofhink.autoloads;
using Theadventureofhink.entities.fire;
using Theadventureofhink.utils;
using Theadventureofhink.world.world_01.hometown.fireplace;

public partial class UndergroundCutscene : Node2D
{
    private Resource _resource = ResourceLoader.Load("res://world/characters/hometown/hometown_villager.dialogue");

    private CutsceneManager _cutsceneManager;
    private Fireplace _fireplace;
    private Node2D _fireArrow;
    private Node2D _fireArrow2;
    private Node2D _fireArrow3;
    private Well _well;
    private Interactible _blacksmith;
    private AnimatedSprite2D _blacksmithSprite;

    public override void _Ready()
    {
        _cutsceneManager = GetNode<CutsceneManager>(Singletons.CutsceneManager);
        _fireplace = GetNode<Fireplace>("Fireplace");
        _fireArrow = GetNode<Node2D>("FireArrow");
        _fireArrow2 = GetNode<Node2D>("FireArrow2");
        _fireArrow3 = GetNode<Node2D>("FireArrow3");
        _well = GetNode<Well>("../../Well");
        _blacksmith = GetNode<Interactible>("BlacksmithCutscene");
        _blacksmithSprite = GetNode<AnimatedSprite2D>("BlacksmithCutscene/AnimatedSprite2D");

        _well.Open();
        _fireplace.Visible = false;
        _fireArrow.Visible = false;
        _fireArrow2.Visible = false;
        _fireArrow3.Visible = false;

        SetAllEnemiesVisible(false);
    }


    public async void StartCutscene()
    {
        GD.Print("Start cutscene!");
        _cutsceneManager.StartDialogue();

        await _cutsceneManager.PlayDialogueCharacterLine("Blacksmith", "Oh, you got my hammer?");
        await _cutsceneManager.PlayDialogueCharacterLine("Hink", "Yes, here you go!");
        await _cutsceneManager.PlayDialogueCharacterLine("Blacksmith",
            "Thanks! Hold on, I'm gonna build the coolest thing!");
        await _cutsceneManager.PlayDialogueCharacterLine("Hink", ".. OK.");
        _blacksmithSprite.Play("working");
        await ToSignal(GetTree().CreateTimer(2.0), "timeout");

        await _cutsceneManager.TransitionOut();
        _fireplace.Visible = true;
        _blacksmithSprite.Play("idle");
        await ToSignal(GetTree().CreateTimer(1.0), "timeout");
        _cutsceneManager.TransitionIn();
        await ToSignal(GetTree().CreateTimer(1.0), "timeout");

        await _cutsceneManager.PlayDialogueCharacterLine("Blacksmith", "Tada!");
        await _cutsceneManager.PlayDialogueCharacterLine("Hink", "Wha... is that a fireplace?");
        await _cutsceneManager.PlayDialogueCharacterLine("Blacksmith", "Yeah! Sweet, huh?");
        await _cutsceneManager.PlayDialogueCharacterLine("Hink", "Yes. But... it is outdoors..?");
        await _cutsceneManager.PlayDialogueCharacterLine("Blacksmith", "Yeah! Sweet, huh? Huh? Huh?");
        await _cutsceneManager.PlayDialogueCharacterLine("Hink", "Why not build it indoors?");
        await _cutsceneManager.PlayDialogueCharacterLine("Blacksmith",
            "Oh, I see. Somebody doesn't wanna enjoy this sweet, cozy fireplace!");
        await _cutsceneManager.PlayDialogueCharacterLine("Blacksmith",
            "I get it, everybody enjoy it, except for Hink!");
        await _cutsceneManager.PlayDialogueCharacterLine("Hink", "...");
        await _cutsceneManager.PlayDialogueCharacterLine("Blacksmith", "You suck!");
        await _cutsceneManager.PlayDialogueCharacterLine("Hink", "Sorry.");
        await _cutsceneManager.PlayDialogueCharacterLine("Woman", "Someone is attacking the village!!");

        await _cutsceneManager.PlayDialogueCharacterLine("Blacksmith", "Huh? What are you talking about?");
        await ShootFireArrows();
        _fireplace.StartFire();
        await ToSignal(GetTree().CreateTimer(1.0), "timeout");
        await _cutsceneManager.PlayDialogueCharacterLine("Blacksmith", "Holy shit! What the hell!");
        await _cutsceneManager.PlayDialogueCharacterLine("Blacksmith", "Hink, hurry! Get need to get to safety!");
        await _cutsceneManager.PlayDialogueCharacterLine("Blacksmith", "I have opened the well, get in it, fast!");
        await _cutsceneManager.PlayDialogueCharacterLine("Hink", "But, what about you guys?");
        await _cutsceneManager.PlayDialogueCharacterLine("Blacksmith", "No time for that Hink, get down the well NOW!");
        _well.Open();
        _cutsceneManager.EndDialogue();
    }

    public async Task ShootFireArrows()
    {
        var arrow1end = _fireArrow.GlobalPosition;
        _fireArrow.GlobalPosition = arrow1end - new Vector2(320, 0);
        _fireArrow.Visible = true;
        var arrow2end = _fireArrow2.GlobalPosition;
        _fireArrow2.GlobalPosition = arrow2end - new Vector2(320, 0);
        _fireArrow2.Visible = true;
        var arrow3end = _fireArrow3.GlobalPosition;
        _fireArrow3.GlobalPosition = arrow3end - new Vector2(320, 0);
        _fireArrow3.Visible = true;

        var tween = CreateTween();
        tween.TweenProperty(_fireArrow, "global_position", arrow1end, 1.0f);
        await ToSignal(tween, "finished");

        var tween2 = CreateTween();
        tween2.TweenProperty(_fireArrow2, "global_position", arrow2end, 1.0f);
        await ToSignal(GetTree().CreateTimer(0.3), "timeout");
        var tween3 = CreateTween();
        tween3.TweenProperty(_fireArrow3, "global_position", arrow3end, 1.0f);
        await ToSignal(tween3, "finished");
    }

    public void OnBodyEnteredFireTrigger(Node2D body)
    {
        GD.Print("FIRE");
        if (CollisionUtil.IsPlayer(body))
        {
            var fires = GetTree().GetNodesInGroup("fires").OfType<HouseFire>().ToList();

            GD.Print("found " + fires.Count() + " fires");
            foreach (var fire in fires)
            {
                fire.State = FireState.OnFire;
            }

            SetAllEnemiesVisible(true);
        }
    }

    public void SetAllEnemiesVisible(bool visible)
    {
        var list = GetTree().GetNodesInGroup("enemies").OfType<Node2D>().ToList();
        foreach (var e in list)
        {
            e.Visible = visible;
        }
    }
}