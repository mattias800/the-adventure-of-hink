extends Node

var state: Dictionary = {
	"player_position": {
		"last_level_name": null,
		"last_portal_name": null,
		"last_checkpoint_name": null
	},
	"levels": {
		"overworld_01": {
			"has_ever_visited": false
		},
		"level_01": {
			"has_ever_visited_room1": false,
			"has_room1_triggered": false,
			"has_ever_visited_room2": false
		},
	},
	"characters": {
		"little_mushroom": {
			"has_met": false
		}
	},
	"player": {
		"can_double_jump": false,
		"can_wall_jump": false,
		"can_climb_walls": false,
		"can_dash": false
	}
}


func once_for_level(level_name: String, field: StringName) -> bool:
	if state["levels"][level_name][field] == false:
		state["levels"][level_name][field] = true
		return true
	else:
		return false

func once_for_character(character_name: String, field: StringName) -> bool:
	if state["characters"][character_name][field] == false:
		state["characters"][character_name][field] = true
		return true
	else:
		return false
