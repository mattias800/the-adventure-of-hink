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
    }

    public void StartLevel()
    {
        var hasDevPortal = GetPortalByName("DevPortal");
        IsEnteringNewScene = true;
        NewScenePortalName = hasDevPortal != null ? "DevPortal" : "StartPortal";
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("respawn"))
        {
            RespawnPlayer();
        }

        if (Input.IsActionJustPressed("exit_game"))
        {
            GD.Print("User pressed Esc, quitting game.");
            GetTree().Quit();
            return;
        }

        if (IsEnteringNewScene)
        {
            GD.Print("Enter new scene.");
            IsEnteringNewScene = false;
            AfterLoadingNewScene();
        }
    }

    public async void OnPlayerEnteredPortal(IPortal portal)
    {
        var nextScenePath = portal.GetNextScenePath();

        if (nextScenePath == null)
        {
            GD.PrintErr("Portal has no scene path set.");
            return;
        }

        LoadNextScene(nextScenePath, portal.GetTargetPortalName());
    }

    public async void LoadNextScene(string nextScenePath, string portalName)
    {
        Player.Disable();
        await _cutsceneManager.TransitionOut();
        await ToSignal(GetTree().CreateTimer(0.5), "timeout");
        IsEnteringNewScene = true;
        NewScenePortalName = portalName;
        LoadScene(nextScenePath);
    }

    public void AfterLoadingNewScene()
    {
        var portal = FindPortalInScene(NewScenePortalName);

        if (portal == null)
        {
            GD.Print("Found no portals at all. Player not spawned.");
            _cutsceneManager.TransitionIn();
            return;
        }

        CurrentCheckpoint = null;
        Player.GlobalPosition = portal.GetSpawnPosition();
        LastSpawnpoint = portal.GetSpawnPosition();

        _cameraManager.SetCameraTarget(Player);
        _cameraManager.Camera?.JumpToTarget();
        _cutsceneManager.TransitionIn();

        Player.PlayerTurned += direction => _cameraManager.Camera?.OnPlayerTurned(direction);

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

        if (!_cutsceneManager.CutscenePlaying)
        {
            // If cutscene was triggered by levels enter room event, it will have disabled the player.
            Player.Enable();
        }

        if (GetTree().CurrentScene.HasMethod("OnPlayerEnterScene"))
        {
            GetTree().CurrentScene.Call("OnPlayerEnterScene");
        }
    }

    private IPortal? FindPortalInScene(string portalName)
    {
        if (!string.IsNullOrEmpty(portalName))
        {
            var portal = GetPortalByName(NewScenePortalName);
            if (portal != null)
            {
                GD.Print("Found specified portal: " + portalName);
                return portal;
            }

            GD.Print("Did not find specified portal: " + portalName);
        }

        return GetAnyAvailablePortal();
    }

    private bool CurrentSceneHasPortal()
    {
        return GetAnyAvailablePortal() != null;
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

    public IPortal? GetAnyAvailablePortal()
    {
        return GetTree().GetFirstNodeInGroup("portals") as IPortal;
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