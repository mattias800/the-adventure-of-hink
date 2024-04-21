extends Node

var has_entered_map := false

@onready var level_manager := %LevelManager
@onready var m : AudioStreamPlayer2D = %Music

func on_player_enter(name: String):
	print("ENTER THE AREA: ", name)
	level_manager.start_timeline("level0exit")
	
	
func on_player_enter_map():
	if not has_entered_map:
		level_manager.play_music("res://assets/audio/music/Early morning guitar.mp3")
		level_manager.start_timeline("intro")
		await Dialogic.timeline_ended
		level_manager.play_music("res://assets/audio/music/hink_the_game.mp3")
		
	has_entered_map = true
