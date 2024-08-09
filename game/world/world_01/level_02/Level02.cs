using Godot;
using Theadventureofhink.autoloads;

public partial class Level02 : Node2D
{
    private Theadventureofhink.autoloads.MusicManager _musicManager;

    public override void _Ready()
    {
        _musicManager = GetNode<Theadventureofhink.autoloads.MusicManager>(Singletons.MusicManager);
        _musicManager.PlayTrack(Tracks.Track.SoftBall);
    }
}