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

        _fireplace.Visible = false;
        _fireArrow.Visible = false;
        _fireArrow2.Visible = false;
        _fireArrow3.Visible = false;

        SetAllEnemiesVisible(false);
    }


    private Task Blacksmith(string say) => _cutsceneManager.PlayDialogueCharacterLine("Blacksmith", say);
    private Task Hink(string say) => _cutsceneManager.PlayDialogueCharacterLine("Hink", say);

    public async void StartCutscene()
    {
        _cutsceneManager.StartDialogue();

        await Blacksmith("Oh, you got my hammer?");
        await Hink("Yes, here you go!");
        await Blacksmith(
            "Thanks! Hold on, I'm gonna build the coolest thing!");
        await Hink(".. OK.");
        _blacksmithSprite.Play("working");
        await ToSignal(GetTree().CreateTimer(2.0), "timeout");

        await _cutsceneManager.TransitionOut();
        _fireplace.Visible = true;
        _blacksmithSprite.Play("idle");
        await ToSignal(GetTree().CreateTimer(1.0), "timeout");
        _cutsceneManager.TransitionIn();
        await ToSignal(GetTree().CreateTimer(1.0), "timeout");

        await Blacksmith("Tada!");
        await Hink("Wha... is that a fireplace?");
        await Blacksmith("Yeah! Sweet, huh?");
        await Hink("Yes. But... it is outdoors..?");
        await Blacksmith("Yeah! Sweet, huh? Huh? Huh?");
        await Hink("Why not build it indoors?");
        await Blacksmith(
            "Oh, I see. Somebody doesn't wanna enjoy this sweet, cozy fireplace!");
        await Blacksmith(
            "I get it, everybody enjoy it, except for Hink!");
        await Hink("...");
        await Blacksmith("You suck!");
        await Hink("Sorry.");
        await _cutsceneManager.PlayDialogueCharacterLine("Woman", "Someone is attacking the village!!");

        await Blacksmith("Huh? What are you talking about?");
        await ShootFireArrows();
        _fireplace.StartFire();
        await ToSignal(GetTree().CreateTimer(1.0), "timeout");
        await Blacksmith("Holy shit! What the hell!");
        await Blacksmith("Hink, hurry! Get need to get to safety!");
        await Blacksmith("I have opened the well, get in it, fast!");
        await Hink("But, what about you guys?");
        await Blacksmith("No time for that Hink, get down the well NOW!");
        _well.IsOpen = true;
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
        if (CollisionUtil.IsPlayer(body))
        {
            var fires = GetTree().GetNodesInGroup("fires").OfType<HouseFire>();
            
            foreach (var fire in fires)
            {
                fire.State = FireState.OnFire;
            }

            SetAllEnemiesVisible(true);
        }
    }

    public void SetAllEnemiesVisible(bool visible)
    {
        var list = GetTree().GetNodesInGroup("enemies").OfType<Node2D>();
        foreach (var e in list)
        {
            e.Visible = visible;
        }
    }
}