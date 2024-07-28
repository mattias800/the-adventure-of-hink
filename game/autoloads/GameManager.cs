using System;
using System.Linq;
using Godot;

namespace Theadventureofhink.autoloads;

public partial class GameManager : Node
{
    public Node2D? CurrentCheckpoint; // TODO Use Checkpoint type.
    public Vector2 LastSpawnpoint;

    public bool IsEnteringNewScene;
    public string NewScenePortalName = "StartPortal";

    public Node2D Player;

    private CameraManager _cameraManager;
    private CutsceneManager _cutsceneManager;

    public override void _Ready()
    {
        Player = GetNode<Node2D>("Player");
        _cameraManager = GetNode<CameraManager>(Singletons.CameraManager);
        _cutsceneManager = GetNode<CutsceneManager>(Singletons.CutsceneManager);
        StartLevel();
        // _cutsceneManager.TransitionIn()
    }

    public void StartLevel()
    {
        IsEnteringNewScene = true;
        NewScenePortalName = "StartPortal";
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("exit_game"))
        {
            GD.Print("User pressed Esc, quitting game.");
            GetTree().Quit();
            return;
        }

        if (IsEnteringNewScene)
        {
            GD.Print("enter new scene!");
            IsEnteringNewScene = false;
            EnterNewScene();
        }
    }

    public async void OnPlayerEnteredPortal(Portal portal)
    {
        GD.Print("on_player_entered_portal");
        Player.Call("disable");
        await _cutsceneManager.TransitionOut();
        await ToSignal(GetTree().CreateTimer(0.5), "timeout");
        IsEnteringNewScene = true;
        NewScenePortalName = portal.TargetPortalName;
        GD.Print("Load next level?");

        if (portal.NextScenePath != null)
        {
            GD.Print("LOAD SCENE WOO");
            load_scene(portal.NextScenePath);
        }
    }

    public void EnterNewScene()
    {
        var portal = get_portal_by_name(NewScenePortalName);

        if (portal == null)
        {
            GD.Print("Found no matching portal: " + NewScenePortalName);
            portal = get_any_available_portal();
        }

        if (portal == null)
        {
            GD.Print("Found no portals at all.");
            GD.Print("Panic!");
            GetTree().Quit();
            return;
        }

        GD.Print("Found portal: " + portal.Name);

        CurrentCheckpoint = null;
        Player.GlobalPosition = portal.GlobalPosition;
        LastSpawnpoint = portal.GlobalPosition;

        GD.Print(Player.GlobalPosition);
        _cameraManager.SetCameraTarget(Player);
        _cameraManager.Camera.JumpToTarget();
        _cutsceneManager.TransitionIn();
        // var camera = get_tree().get_first_node_in_group("cameras")
        
        Player.Connect("player_turned", new Callable(this, nameof(OnPlayerTurned)));
        
        if (GetTree().CurrentScene.IsInGroup("platformers"))
        {
            Player.Call("switch_to_platform");
        }
        else if (GetTree().CurrentScene.IsInGroup("overworlds"))
        {
            Player.Call("switch_to_overworld");
        }
        else
        {
            GD.Print("Scene must be in group platformers or overworlds.");
            GetTree().Quit();
        }

        if (!_cutsceneManager.Get("cutscene_playing").AsBool())
        {
            // If cutscene was triggered by levels enter room event, it will have disabled the player.
            GD.Print("gameManager enabling player!!");
            Player.Call("enable");
        }

        if (GetTree().CurrentScene.HasMethod("on_player_enter_scene"))
        {
            GetTree().CurrentScene.Call("on_player_enter_scene");
        }
    }

    private void OnPlayerTurned(string direction)
    {
        _cameraManager.Camera.OnPlayerTurned(direction);
    }
    
    private Portal? get_portal_by_name(string name)
    {
        var portals = GetTree().GetNodesInGroup("portals").OfType<Portal>();

        foreach (var p in portals)
        {
            if (p.Name == name)
            {
                return p;
            }
        }

        return null;
    }

    public Portal? get_any_available_portal()
    {
        return GetTree().GetFirstNodeInGroup("portals") as Portal;
    }

    public void OnCutsceneManagerCutsceneStarted()
    {
        Player.Call("disable");
    }

    public void OnCutsceneManagerCutsceneEnded()
    {
        Player.Call("enable");
    }

    public void respawn_player()
    {
        if (CurrentCheckpoint != null)
        {
            Player.Call("death_teleport", CurrentCheckpoint.GlobalPosition);
        }
        else if (LastSpawnpoint != null)
        {
            Player.Call("death_teleport", LastSpawnpoint);
        }
        else
        {
            GD.Print("Respawn failed. No checkpoint and no last known spawnpoint.");
            GetTree().Quit();
        }
    }

    public void load_scene(string path)
    {
        GD.Print("LOADING NEW SCENE!");
        GetTree().CurrentScene.QueueFree(); // Instead of free()
        var packedScene = ResourceLoader.Load(path) as PackedScene;
        if (packedScene == null)
        {
            throw new Exception("ResourceLoader got null when loading packed scene.");
        }

        var instancedScene = packedScene.Instantiate();
        // Add it to the scene tree, as direct child of root
        GetTree().Root.AddChild(instancedScene);
        //  Set it as the current scene, only after it has been added to the tree
        GetTree().CurrentScene = instancedScene;
    }
}