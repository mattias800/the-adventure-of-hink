using Godot;
using Theadventureofhink.autoloads;

public partial class Level02 : Node2D
{
    private MusicManager _musicManager;

    public override void _Ready()
    {
        _musicManager = GetNode<MusicManager>(Singletons.MusicManager);
        _musicManager.PlayTrack(Tracks.Track.SoftBall);
    }
}