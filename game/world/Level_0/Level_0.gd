extends Node

var has_entered_map := false

var watched = {
	"level0exit": false,
	"level0start": false
}

@onready var level_manager := %LevelManager
@onready var cutscene_manager := %CutsceneManager
@onready var music_manager := %MusicManager

func on_player_enter_map():
	if watched["level0start"] == false:
		music_manager.play_track(MusicManager.Track.EARLY_MORNING)
		cutscene_manager.start_timeline("level0start")
	watched["level0start"] = true

func on_player_enter(trigger_name: String):
	match trigger_name:
		"level0exit":
			if watched["level0exit"] == false:
				cutscene_manager.start_timeline("level0exit")
			watched["level0exit"] = true
