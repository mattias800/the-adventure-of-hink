using System;
using System.Linq;
using Godot;
using Theadventureofhink.entities.portals;

namespace Theadventureofhink.autoloads;

public partial class GameManager : Node
{
    public Node2D? CurrentCheckpoint; // TODO Use Checkpoint type.
    public Vector2 LastSpawnpoint;

    public bool IsEnteringNewScene;
    public string NewScenePortalName = "StartPortal";

    public Player Player;

    private CameraManager _cameraManager;
    private CutsceneManager _cutsceneManager;

    public override void _Ready()
    {
        Player = GetNode<Player>("Player");
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

    public async void OnPlayerEnteredPortal(IPortal portal)
    {
        GD.Print("on_player_entered_portal");
        Player.Disable();
        await _cutsceneManager.TransitionOut();
        await ToSignal(GetTree().CreateTimer(0.5), "timeout");
        IsEnteringNewScene = true;
        NewScenePortalName = portal.GetTargetPortalName();
        
        GD.Print("Next portal: " + NewScenePortalName);
        GD.Print("Load next level?");

        var nextScenePath = portal.GetNextScenePath();
        
        if (nextScenePath != null)
        {
            GD.Print("LOAD SCENE WOO");
            LoadScene(nextScenePath);
        }
    }

    public void EnterNewScene()
    {
        var portal = GetPortalByName(NewScenePortalName);

        if (portal == null)
        {
            GD.Print("Found no matching portal: " + NewScenePortalName);
            portal = GetAnyAvailablePortal();
        }

        if (portal == null)
        {
            GD.Print("Found no portals at all.");
            GD.Print("Panic!");
            GetTree().Quit();
            return;
        }

        GD.Print("Found portal: " + portal.GetName());

        CurrentCheckpoint = null;
        Player.GlobalPosition = portal.GetSpawnPosition();
        LastSpawnpoint = portal.GetSpawnPosition();

        GD.Print(Player.GlobalPosition);
        _cameraManager.SetCameraTarget(Player);
        _cameraManager.Camera.JumpToTarget();
        _cutsceneManager.TransitionIn();
        // var camera = get_tree().get_first_node_in_group("cameras")

        Player.Connect("player_turned", new Callable(this, nameof(OnPlayerTurned)));

        if (GetTree().CurrentScene.IsInGroup("platformers"))
        {
            Player.SwitchToPlatform();
        }
        else if (GetTree().CurrentScene.IsInGroup("overworlds"))
        {
            Player.SwitchToOverworld();
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
            Player.Enable();
        }

        if (GetTree().CurrentScene.HasMethod("OnPlayerEnterScene"))
        {
            GetTree().CurrentScene.Call("OnPlayerEnterScene");
        }
    }

    private void OnPlayerTurned(string direction)
    {
        _cameraManager.Camera.OnPlayerTurned(direction);
    }

    private IPortal? GetPortalByName(string name)
    {
        var portals = GetTree().GetNodesInGroup("portals").OfType<IPortal>();

        foreach (var p in portals)
        {
            if (p.GetName() == name)
            {
                return p;
            }
        }

        return null;
    }

    public PortalArea? GetAnyAvailablePortal()
    {
        return GetTree().GetFirstNodeInGroup("portals") as PortalArea;
    }

    public void OnCutsceneManagerCutsceneStarted()
    {
        Player.Disable();
    }

    public void OnCutsceneManagerCutsceneEnded()
    {
        Player.Enable();
    }

    public void RespawnPlayer()
    {
        GD.Print("RespawnPlayer");
        if (CurrentCheckpoint != null)
        {
            Player.DeathTeleport(CurrentCheckpoint.GlobalPosition);
        }
        else if (LastSpawnpoint != null)
        {
            Player.DeathTeleport(LastSpawnpoint);
        }
        else
        {
            GD.Print("Respawn failed. No checkpoint and no last known spawn point.");
            GetTree().Quit();
        }
    }

    public void LoadScene(string path)
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