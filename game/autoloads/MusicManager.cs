using Godot;
using System;
using Theadventureofhink.autoloads;

public partial class MusicManager : Node
{
    private AudioStreamPlayer _player;
    private Tracks _tracks;

    private Tracks.Track? _currentlyPlayingTrack;
    private Tracks.Track? _queuedTrack;

    public override void _Ready()
    {
        _player = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        _tracks = GetNode<Tracks>("Tracks");
    }
    
    public override void _Process(double delta)
    {
        if (_queuedTrack != null)
        {
            PlayQueuedTrack();
            _queuedTrack = null;
        }
    }

    public void PlayTrack(Tracks.Track track)
    {
        if (_currentlyPlayingTrack == track && _player.Playing)
        {
            return;
        }

        _queuedTrack = track;
    }

    public void PlayQueuedTrack()
    {
        if (_player.Playing)
        {
            // TODO Fade out
            _player.Stop();
        }

        _currentlyPlayingTrack = _queuedTrack;
        if (_currentlyPlayingTrack != null)
        {
            var audioStreamPlayer = _tracks.GetTrackNode(_currentlyPlayingTrack.Value);
            if (audioStreamPlayer != null)
            {
                PlayTrackNode(audioStreamPlayer);
            }
        }
    }

    private void PlayTrackNode(AudioStreamPlayer trackNode)
    {
        _player.Stream = trackNode.Stream;
        _player.Play();
    }
}