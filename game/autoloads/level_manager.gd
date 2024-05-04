extends Node

const LdtkUtil = preload("res://src/utils/LdtkUtil.gd")

signal player_exited_level
signal player_entered_level
signal player_entered_portal
signal player_entered_out_of_bounds

var active_room_tilemap
var is_out_of_bounds: bool = false

enum LevelType {
	PLATFORM,
	OVERWORLD
}

func check_for_level_change(player_world_position: Vector2):
	return
	var room_tilemap = LdtkUtil.find_level_tilemap_by_world_coordinates(get_tree(), player_world_position)
	
	if room_tilemap is TileMap:
		is_out_of_bounds = false
		if (room_tilemap != active_room_tilemap):
			if active_room_tilemap:
				pass
			active_room_tilemap = room_tilemap
	else:
		if not is_out_of_bounds:
			player_entered_out_of_bounds.emit()
		is_out_of_bounds = true

func get_level_type_for_tilemap(room_tilemap: TileMap):
	if room_tilemap.is_in_group("rooms"):
		return LevelType.PLATFORM
	if room_tilemap.is_in_group("overworlds"):
		return LevelType.OVERWORLD
		

