extends Node

var grandpa = {}
var blacksmith = {}
var villagers = {}
var mushy = {}
var first_times = {}

func first_time(k: String) -> bool:
	if not first_times.get(k):
		first_times[k] = true
		return true
	return false
	
func player_can_double_jump():
	return DialogueStateHelper.PlayerCanDoubleJump()
	
func player_can_wall_jump():
	return DialogueStateHelper.PlayerCanWallJump()
	
func player_can_wall_climb():
	return DialogueStateHelper.PlayerCanClimbWalls()
	
func player_can_dash():
	return DialogueStateHelper.PlayerCanDash()

func player_has_blacksmiths_hammer():
	return DialogueStateHelper.PlayerHasBlacksmithsHammer()
