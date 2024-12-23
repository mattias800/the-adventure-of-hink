using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Theadventureofhink.entities.portals;
using Theadventureofhink.game_saves;
using Theadventureofhink.game_state;
using Theadventureofhink.world;

namespace Theadventureofhink.autoloads;

public partial class GameManager : Node
{
    public Node2D CurrentCheckpoint; // TODO Use Checkpoint type.
    public Vector2 LastSpawnPoint;

    public bool IsEnteringNewScene;
    public string NewScenePortalName = "StartPortal";
    public string NewCheckpointName = null;

    public features.player.Player Player;

    private CameraManager _cameraManager;
    private CutsceneManager _cutsceneManager;
    private GameStateManager _gameStateManager;
    private GameSaveSlotManager _gameSaveSlotManager;

    public override void _Ready()
    {
        Player = GetNode<features.player.Player>("Player");
        _cameraManager = GetNode<CameraManager>(Singletons.CameraManager);
        _cutsceneManager = GetNode<CutsceneManager>(Singletons.CutsceneManager);
        _gameStateManager = GetNode<GameStateManager>(Singletons.GameStateManager);
        _gameSaveSlotManager = GetNode<GameSaveSlotManager>(Singletons.GameSaveSlotManager);

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
        _gameStateManager.IncreaseTimePlayed(delta);

        if (Input.IsActionJustPressed("respawn"))
        {
            RespawnPlayer();
        }

        if (Input.IsActionJustPressed("exit_game"))
        {
            GD.Print("User pressed Esc, quitting game.");
            Callable.From(() => GetTree().Quit()).CallDeferred();
            return;
        }

        if (IsEnteringNewScene)
        {
            GD.Print("Enter new scene.");
            IsEnteringNewScene = false;
            AfterLoadingNewScene();
        }
    }

    public void OnPlayerEnteredPortal(IPortal portal)
    {
        LoadNextStageAndSave(portal.GetNextStage(), portal.GetTargetPortalName());
    }

    public void LoadNextStageAndSave(Stage stage, string portalName)
    {
        SetStageAndPortalInGameState(stage, portalName);
        _gameSaveSlotManager.SaveGameToCurrentSlot();
        _ = LoadNextScene(Stages.GetStateInfo(stage).FilePath, portalName);
    }

    public void LoadNextStageStoredInGameState()
    {
        var scenePortalName = _gameStateManager.GameState.PlayerPositionState.LastStage;
        var portalName = _gameStateManager.GameState.PlayerPositionState.LastPortalName;
        NewCheckpointName = _gameStateManager.GameState.PlayerPositionState.LastCheckpointName;
        _ = LoadNextScene(Stages.GetStateInfo((Stage)scenePortalName).FilePath, portalName);
    }

    public void SetCurrentCheckpointAndSave(Checkpoint checkpoint)
    {
        CurrentCheckpoint = checkpoint;
        _gameStateManager.GameState.PlayerPositionState.LastCheckpointName = checkpoint.Name;
        _gameSaveSlotManager.SaveGameToCurrentSlot();
    }

    public void SetStageAndPortalInGameState(Stage stage, string portalName)
    {
        _gameStateManager.GameState.PlayerPositionState.LastStage = (int)stage;
        _gameStateManager.GameState.PlayerPositionState.LastPortalName = portalName;
        _gameStateManager.GameState.PlayerPositionState.LastCheckpointName = "";
    }

    private async Task LoadNextScene(string nextScenePath, string portalName)
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
            _cutsceneManager.TransitionIn(CutsceneManager.TransitionFocus.Center);
            return;
        }

        var checkpoint = NewCheckpointName != null ? GetCheckpointByName(NewCheckpointName) : null;

        var nextPlayerPosition = checkpoint?.GlobalPosition ?? portal.GetSpawnPosition();

        CurrentCheckpoint = checkpoint;
        Player.Velocity = Vector2.Zero;
        Player.GlobalPosition = nextPlayerPosition;
        LastSpawnPoint = nextPlayerPosition;
        NewCheckpointName = null;

        _cameraManager.SetCameraTarget(Player);
        _cameraManager.Camera?.JumpToTarget();
        _cutsceneManager.TransitionIn(CutsceneManager.TransitionFocus.Player);

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

        // Player was just moved, so if we enable the player, it will trigger OnPlayerExitRoom, etc.
        // We defer the call to allow the player to be moved first.
        GetTree().CreateTimer(0.5f).Timeout += EnablePlayerIfNoCutscene;

        if (GetTree().CurrentScene.HasMethod("OnPlayerEnterScene"))
        {
            GetTree().CurrentScene.Call("OnPlayerEnterScene");
        }
    }

    private void EnablePlayerIfNoCutscene()
    {
        if (!_cutsceneManager.CutscenePlaying)
        {
            Player.Enable();
        }
    }

    private IPortal FindPortalInScene(string portalName)
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

    private IPortal GetPortalByName(string name)
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

    private Checkpoint GetCheckpointByName(string name)
    {
        var checkpoints = GetTree().GetNodesInGroup("checkpoints").OfType<Checkpoint>();

        foreach (var p in checkpoints)
        {
            if (p.GetName() == name)
            {
                return p;
            }
        }

        return null;
    }

    public IPortal GetAnyAvailablePortal()
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
        GetNode<GameStateManager>(Singletons.GameStateManager).IncreaseNumberOfPlayerDeaths();

        GD.Print("RespawnPlayer");
        if (CurrentCheckpoint != null)
        {
            Player.DeathTeleport(CurrentCheckpoint.GlobalPosition);
        }
        else if (LastSpawnPoint != null)
        {
            Player.DeathTeleport(LastSpawnPoint);
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