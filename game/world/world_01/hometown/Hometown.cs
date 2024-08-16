using Godot;
using Theadventureofhink.autoloads;
using Theadventureofhink.game_state;
using Theadventureofhink.utils;

public partial class Hometown : Node2D
{
    private Room _room;
    private Room _room2;
    private Room _room1Underground;
    private Room _room2Underground;
    private UndergroundCutscene _undergroundCutscene;
    private HometownVillager _blacksmith;

    private MusicManager _musicManager;
    private GameStateManager _gameStateManager;

    public override void _Ready()
    {
        _musicManager = GetNode<MusicManager>(Singletons.MusicManager);
        _gameStateManager = GetNode<GameStateManager>(Singletons.GameStateManager);

        _room = GetNode<Room>("Rooms/Room");
        _room2 = GetNode<Room>("Rooms/Room2");
        _room1Underground = GetNode<Room>("Rooms/Room1Underground");
        _room2Underground = GetNode<Room>("Rooms/Room2Underground");
        _undergroundCutscene = GetNode<UndergroundCutscene>("States/UndergroundCutscene");
        _blacksmith = GetNode<HometownVillager>("States/StartState/Blacksmith");

        if (!_gameStateManager.GameState.PlayerState.PlayerItemsState.GotBlacksmithsHammers.Value)
        {
            _undergroundCutscene.QueueFree();
        }
        else
        {
            _blacksmith.QueueFree();
        }

        _musicManager.PlayTrack(Tracks.Track.SoftBall);
    }

    public void OnUndergroundCameraTriggered(Node2D body)
    {
        if (CollisionUtil.IsPlayer(body))
        {
            _room.Enabled = false;
            _room2.Enabled = false;
            _room1Underground.Enabled = true;
            _room2Underground.Enabled = true;
        }
    }

    public void OnOvergroundCameraTriggered(Node2D body)
    {
        if (CollisionUtil.IsPlayer(body))
        {
            _room.Enabled = true;
            _room2.Enabled = true;
            _room1Underground.Enabled = false;
            _room2Underground.Enabled = false;
        }
    }
}