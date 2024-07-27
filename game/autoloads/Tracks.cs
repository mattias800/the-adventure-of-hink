using Godot;

namespace Theadventureofhink.autoloads;

public partial class Tracks : Node
{
    public enum Track
    {
        EarlyMorning,
        HinkTheGame,
        SoftBall,
        WhisperingShadows
    }

    public AudioStreamPlayer? GetTrackNode(Track song)
    {
        switch (song)
        {
            case Track.EarlyMorning:
                return GetNode<AudioStreamPlayer>("EarlyMorningMusic");
            case Track.HinkTheGame:
                return GetNode<AudioStreamPlayer>("HinkTheGameMusic");
            case Track.SoftBall:
                return GetNode<AudioStreamPlayer>("SoftBall");
            case Track.WhisperingShadows:
                return GetNode<AudioStreamPlayer>("WhisperingShadows");
            default:
                return null;
        }
    }
}