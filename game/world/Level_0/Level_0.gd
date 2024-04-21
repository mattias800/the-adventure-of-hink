extends Node

var has_entered_map := false

@onready var level_manager := %LevelManager
@onready var m : AudioStreamPlayer2D = %Music

func on_player_enter_map():
	if not has_entered_map:
		level_manager.start_timeline("level0start")		
	has_entered_map = true

func on_player_enter(name: String):
	match name:
		"level0exit":
			level_manager.start_timeline("level0exit")
	
