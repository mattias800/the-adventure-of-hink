extends Node

signal player_left_tilemap
signal player_entered_tilemap

const TileMapByCoordinateFinder = preload("res://TileMapByCoordinateFinder.gd")

var active_tile_map: TileMap
var active_level_name: String

@onready var player := %Player
signal cutscene_started
signal cutscene_ended
@onready var m: AudioStreamPlayer2D = %Music


func _ready():
	pass


func _process(delta):
	check_for_level_change(player.global_position)


func check_for_level_change(player_world_position: Vector2):
	var result := TileMapByCoordinateFinder.find_tilemap_by_world_coordinates(get_tree().root, player_world_position)
	match result:
		[true, var tilemap, var level_name]:
			if (tilemap != active_tile_map):
				if (active_tile_map):
					player_leave_map(active_level_name, active_tile_map)
				active_tile_map = tilemap
				active_level_name = level_name
				player_enter_map(active_level_name, active_tile_map)

		[false, _]:
			print("Could not find tilemap containing player.")


func player_enter_map(level_name: String, tilemap: TileMap):
	print("player_enter_map", level_name)
	load_entities(level_name)
	var level_controller = get_node(level_name)
	if level_controller:
		level_controller.on_player_enter_map()
	player_entered_tilemap.emit(tilemap.get_parent().name)

func player_leave_map(level_name: String, tilemap: TileMap):
	print("player_leave_map", level_name)
	# unload_entities(level_name)
	var level_controller = get_node(level_name)
	if level_controller:
		level_controller.on_player_enter_map()
	player_left_tilemap.emit(tilemap.get_parent().name)


func play_music(path):
	m.stream = load_mp3(path)
	m.play()


func load_mp3(path) -> AudioStreamMP3:
	var file  := FileAccess.open(path, FileAccess.READ)
	var sound := AudioStreamMP3.new()
	sound.data = file.get_buffer(file.get_length())
	return sound


func start_timeline(name: String):
	cutscene_started.emit()
	Dialogic.start("intro")
	await Dialogic.timeline_ended
	await get_tree().create_timer(0.1).timeout # Prevent last input to be sent to player.
	cutscene_ended.emit()


func load_entities(level_name: String) -> void:
	var entities := TileMapByCoordinateFinder.find_entities_meta_for_level(get_tree().root, level_name)
	for entity in entities:
		load_entity(entity)

func load_entity(entity: Dictionary):
	print("load entity: " + entity.identifier)
	match entity.identifier:
		"OnPlayerEnter":
			pass
		"SpawnPlayer":
			pass
