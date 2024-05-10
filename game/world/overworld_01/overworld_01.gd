extends Node2D

@onready var resource = load("res://world/overworld_01/overworld_01.dialogue")

func _ready():
	MusicManager.play_track(Tracks.Track.WHISPERING_SHADOWS)

func on_player_enter_scene():
	if GameState.once_for_level("overworld_01", "has_ever_visited"):
		CutsceneManager.start_timeline(resource, "first_entry")
