extends Node

var grandpa = {}
var blacksmith = {}
var mushy = {}

func player_can_double_jump():
	return DialogueStateHelper.PlayerCanDoubleJump()
	
func player_can_wall_jump():
	return DialogueStateHelper.PlayerCanWallJump()
	
func player_can_climb_walls():
	return DialogueStateHelper.PlayerCanClimbWalls()
	
func player_can_dash():
	return DialogueStateHelper.PlayerCanDash()
