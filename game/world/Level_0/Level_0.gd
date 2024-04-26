extends Node

var has_entered_map := false

var watched = {
	"level0exit": false,
	"level0start": false
}

@onready var level_manager := %LevelManager
@onready var m : AudioStreamPlayer2D = %Music
@onready var music := %Music

func on_player_enter_map():
	pass
	#if watched["level0start"] == false:
#		music.play_music(music.songs.early_morning)
#		# level_manager.play_music("res://assets/audio/music/early_morning_guitar.mp3")
#		level_manager.start_timeline("level0start")		
#	watched["level0start"] = true

func on_player_enter(trigger_name: String):
	match trigger_name:
		"level0exit":
			if watched["level0exit"] == false:
				level_manager.start_timeline("level0exit")
			watched["level0exit"] = true
