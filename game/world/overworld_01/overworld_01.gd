extends Node2D

func _ready():
	MusicManager.play_track(Tracks.Track.WHISPERING_SHADOWS)

func on_player_enter_scene():
	var resource = load("res://world/overworld_01/overworld_01.dialogue")
	CutsceneManager.start_timeline(resource, "first_entry")
