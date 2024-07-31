extends Node

var grandpa = {}
var mushy = {}

func player_can_double_jump():
	print("GameState")
	print(str(GameState))
	return GameState.PlayerState.PlayerSkillsState.CanDoubleJump.Value()
	
func player_can_wall_jump():
	return true
func player_can_climb_walls():
	return true
func player_can_dash():
	return true
