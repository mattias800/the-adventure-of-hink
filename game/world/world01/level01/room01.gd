extends Node

var has_entered_map := false

func on_player_enter():
	if not has_entered_map:
		CutsceneManager.start_timeline("intro")
		await Dialogic.timeline_ended
		MusicManager.play_track(MusicManager.Track.HINK_THE_GAME)
		
	has_entered_map = true
