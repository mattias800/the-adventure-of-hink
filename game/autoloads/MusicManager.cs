using Godot;

namespace Theadventureofhink.autoloads;

public partial class MusicManager : Node
{
    public bool IsPlaying;
    
    private Tracks _tracks;

    private Tracks.Track? _currentlyPlayingTrack;
    private Tracks.Track? _queuedTrack;
    
    public override void _Ready()
    {
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

    private void OnMusicStopped()
    {
        if (IsPlaying && _currentlyPlayingTrack != null)
        {
            var audioStreamPlayer = _tracks.GetTrackNode(_currentlyPlayingTrack.Value);
            audioStreamPlayer?.Play();
        }
    }

    public void Stop()
    {
        IsPlaying = false;
        StopCurrentlyPlaying();
        _currentlyPlayingTrack = null;
    }

    public void PlayTrack(Tracks.Track track)
    {
        if (_currentlyPlayingTrack == track && IsPlaying)
        {
            return;
        }

        _queuedTrack = track;
    }

    private void PlayQueuedTrack()
    {
        StopCurrentlyPlaying();
        
        if (_queuedTrack != null)
        {
            var audioStreamPlayer = _tracks.GetTrackNode(_queuedTrack.Value);
            if (audioStreamPlayer != null)
            {
                audioStreamPlayer.Play();
                audioStreamPlayer.Finished += OnMusicStopped;
                IsPlaying = true;
            }
            else
            {
                IsPlaying = false;
            }
        }
        
        _currentlyPlayingTrack = _queuedTrack;
    }

    private void StopCurrentlyPlaying()
    {
        if (_currentlyPlayingTrack != null)
        {
            var audioStreamPlayer = _tracks.GetTrackNode(_currentlyPlayingTrack.Value);
            if (audioStreamPlayer != null)
            {
                audioStreamPlayer.Finished -= OnMusicStopped;
                audioStreamPlayer.Stop();
            }
        }
    }
}