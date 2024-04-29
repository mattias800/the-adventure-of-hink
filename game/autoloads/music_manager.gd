extends Node

@onready var player := $AudioStreamPlayer
@onready var tracks := $Tracks

var currently_playing_track

func play_track(track: Tracks.Track):
	if currently_playing_track == track and player.playing:
		# It is already playing
		return

	if player.playing:
		# Another song is playing, fade into new one.
		# TODO Fade
		player.stop()
	
	currently_playing_track = track
	play_track_node(tracks.get_track_node(track))
	
func play_track_node(track_node: AudioStreamPlayer):
	player.stream = track_node.stream
	player.play()
