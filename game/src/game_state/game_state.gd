extends Node

var state = {
	"overworld_01": {
		"has_ever_visited": false
	},
	"level_01": {
		"has_ever_visited_room1": false,
		"has_room1_triggered": false,
		"has_ever_visited_room2": false
	}
} 

func once(level_name: String, field: StringName):
	if state[level_name][field] == false:
		state[level_name][field] = true
		return true
	else:
		return false
