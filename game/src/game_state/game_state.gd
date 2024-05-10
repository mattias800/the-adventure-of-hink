extends Node

var state = {
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
				}
			}


func once_for_level(level_name: String, field: StringName):
	if state["levels"][level_name][field] == false:
		state["levels"][level_name][field] = true
		return true
	else:
		return false

func once_for_character(character_name: String, field: StringName):
	if state["characters"][character_name][field] == false:
		state["characters"][character_name][field] = true
		return true
	else:
		return false
