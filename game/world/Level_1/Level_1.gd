extends Node

var has_entered_map := false

@onready var level_manager := %LevelManager
@onready var m : AudioStreamPlayer2D = %Music

func on_player_enter_map():
	if not has_entered_map:
		level_manager.start_timeline("intro")
		await Dialogic.timeline_ended
		level_manager.play_music("res://assets/audio/music/hink_the_game.mp3")
		
	has_entered_map = true
