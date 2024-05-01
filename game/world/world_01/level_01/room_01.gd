extends TileMap

var has_entered_room := false

func on_player_enter_room():
	if not has_entered_room:
		var resource = load("res://world/world_01/level_01/level_01.dialogue")
		CutsceneManager.start_timeline(resource, "first_entry")
		

	has_entered_room = true
