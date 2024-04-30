extends TileMap

var has_entered_room := false

func on_player_enter_room():
	if not has_entered_room:
		CutsceneManager.start_timeline("intro")
		await Dialogic.timeline_ended
		MusicManager.play_track(Tracks.Track.HINK_THE_GAME)
		
	has_entered_room = true
