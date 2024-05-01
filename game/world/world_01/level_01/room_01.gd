extends TileMap

var has_entered_room := false

func on_player_enter_room():
	if not has_entered_room:
		#CutsceneManager.start_timeline("level0start")
		#await Dialogic.timeline_ended

	has_entered_room = true
