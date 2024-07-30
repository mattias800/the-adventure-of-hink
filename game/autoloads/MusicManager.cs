using Godot;
using System;
using Theadventureofhink.autoloads;

public partial class MusicManager : Node
{
    private AudioStreamPlayer _player;
    private Tracks _tracks;

    private Tracks.Track? _currentlyPlayingTrack;
    private Tracks.Track? _queuedTrack;

    private bool _isPlaying;

    public override void _Ready()
    {
        _player = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        _tracks = GetNode<Tracks>("Tracks");

        _player.Finished += OnMusicStopped;
    }
    
    public override void _Process(double delta)
    {
        if (_queuedTrack != null)
        {
            PlayQueuedTrack();
            _queuedTrack = null;
        }
    }

    private void OnMusicStopped()
    {
        if (_isPlaying)
        {
            _player.Play();
        }
    }

    public void Stop()
    {
        _isPlaying = false;
        _player.Stop();
        _player.Stream = null;
        _currentlyPlayingTrack = null;
    }

    public void PlayTrack(Tracks.Track track)
    {
        if (_currentlyPlayingTrack == track && _player.Playing)
        {
            return;
        }

        _queuedTrack = track;
    }

    private void PlayQueuedTrack()
    {
        if (_isPlaying)
        {
            // TODO Fade out
            Stop();
        }

        _currentlyPlayingTrack = _queuedTrack;
        if (_currentlyPlayingTrack != null)
        {
            var audioStreamPlayer = _tracks.GetTrackNode(_currentlyPlayingTrack.Value);
            if (audioStreamPlayer != null)
            {
                PlayTrackNode(audioStreamPlayer);
                _isPlaying = true;
            }
        }
    }

    private void PlayTrackNode(AudioStreamPlayer trackNode)
    {
        _player.Stream = trackNode.Stream;
        _player.Play();
    }
}