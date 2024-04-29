extends Node

var has_entered_map := false

var watched = {
	"level0exit": false,
	"level0start": false
}

func on_player_enter_map():
	if watched["level0start"] == false:
		MusicManager.play_track(Tracks.Track.EARLY_MORNING)
		CutsceneManager.start_timeline("level0start")
	watched["level0start"] = true

func on_player_enter(trigger_name: String):
	match trigger_name:
		"level0exit":
			if watched["level0exit"] == false:
				CutsceneManager.start_timeline("level0exit")
			watched["level0exit"] = true
