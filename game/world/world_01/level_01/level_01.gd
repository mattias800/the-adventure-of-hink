extends Node

@onready var resource = load("res://world/world_01/level_01/level_01.dialogue")

func _ready():
	MusicManager.play_track(Tracks.Track.EARLY_MORNING)

func _on_room_1_on_player_entered_room():
	if GameState.once_for_level("level_01", "has_ever_visited_room1"):
		CutsceneManager.start_timeline(resource, "room1_entry")

func _on_room_2_on_player_entered_room():
	if GameState.once_for_level("level_01", "has_room1_triggered"):
		CutsceneManager.start_timeline(resource, "room2_entry")

func _on_room_3_on_player_entered_room():
	pass

func _on_room_1_trigger_on_player_entered():
	if GameState.once_for_level("level_01", "has_ever_visited_room2"):
		CutsceneManager.start_timeline(resource, "room1_trigger")
