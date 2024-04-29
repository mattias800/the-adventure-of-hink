extends Node

const LdtkUtil = preload("res://src/utils/LdtkUtil.gd")

signal player_exited_level
signal player_entered_level
signal player_entered_portal
signal player_entered_out_of_bounds

var active_level_tilemap: TileMap
var is_out_of_bounds: bool = false

enum LevelType {
	PLATFORM,
	OVERWORLD
}

func check_for_level_change(player_world_position: Vector2):
	var level_tilemap = LdtkUtil.find_level_tilemap_by_world_coordinates(get_tree(), player_world_position)
	
	if level_tilemap is TileMap:
		is_out_of_bounds = false
		if (level_tilemap != active_level_tilemap):
			if active_level_tilemap:
				player_leave_level(active_level_tilemap)
			active_level_tilemap = level_tilemap
			player_enter_level(active_level_tilemap)
	else:
		if not is_out_of_bounds:
			player_entered_out_of_bounds.emit()
		is_out_of_bounds = true

func get_level_type_for_tilemap(room_tilemap: TileMap):
	if room_tilemap.is_in_group("rooms"):
		return LevelType.PLATFORM
	if room_tilemap.is_in_group("overworlds"):
		return LevelType.OVERWORLD
		
func player_enter_level(level_tilemap: TileMap):
	print("player_enter_level: ", level_tilemap.name)
	# load_entities(room_tilemap)
	
	if level_tilemap.has_method("on_player_enter"):
		level_tilemap.on_player_enter()

	print("player_entered_tilemap.emit")
	player_entered_level.emit(level_tilemap)

func player_leave_level(level_tilemap: TileMap):
	print("player_leave_level", level_tilemap.name)
	# unload_entities(level_name)
	
	if level_tilemap.has_method("on_player_leave"):
		level_tilemap.on_player_leave()

	player_exited_level.emit(level_tilemap)
