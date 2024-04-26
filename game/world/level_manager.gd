extends Node

signal player_left_tilemap
signal player_entered_tilemap
const tile_map_by_coordinate_finder = preload("res://tile_map_by_coordinate_finder.gd")
var active_tile_map: TileMap
var active_level_name: String

signal cutscene_started
signal cutscene_ended
@onready var m: AudioStreamPlayer2D = %Music

enum LevelType {
	PLATFORM,
	OVERWORLD
}

var player


func _ready():
	pass


func _process(_delta):
	if player:
		check_for_level_change(player.global_position)

func set_current_player(p: Node2D):
	player = p

func check_for_level_change(player_world_position: Vector2):
	var result := tile_map_by_coordinate_finder.find_tilemap_by_world_coordinates(get_tree().root, player_world_position)
	match result:
		[true, var tilemap, var level_name]:
			if (tilemap != active_tile_map):
				if (active_tile_map):
					player_leave_map(active_level_name, active_tile_map)
				active_tile_map = tilemap
				active_level_name = level_name
				player_enter_map(active_level_name, active_tile_map)

		[false, _, _]:
			print("Could not find tilemap containing player.")


func player_enter_map(level_name: String, tilemap: TileMap):
	print("player_enter_map: ", level_name, tilemap.name)
	load_entities(level_name)
	var level_controller = get_node(level_name)
	if level_controller and level_controller.has_method("on_player_enter_map"):
		level_controller.on_player_enter_map()
		
	var level_node = tilemap.get_parent()	
	player_entered_tilemap.emit(level_node.name, tilemap, get_level_metadata(level_node))

func get_level_metadata(level_node: Node2D):
	return level_node.get_meta("LDtk_level_fields")
	
func player_leave_map(level_name: String, tilemap: TileMap):
	print("player_leave_map", level_name)
	# unload_entities(level_name)
	var level_controller = get_node(level_name)
	if level_controller and level_controller.has_method("on_player_leave_map"):
		level_controller.on_player_leave_map()
		
	var level_node = tilemap.get_parent()
	player_left_tilemap.emit(level_node.name, tilemap, get_level_metadata(level_node))


func play_music(path):
	m.stream = load_mp3(path)
	m.play()


func load_mp3(path) -> AudioStreamMP3:
	var file  := FileAccess.open(path, FileAccess.READ)
	var sound := AudioStreamMP3.new()
	sound.data = file.get_buffer(file.get_length())
	return sound


func start_timeline(timeline_name: String):
	cutscene_started.emit()
	Dialogic.start(timeline_name)
	await Dialogic.timeline_ended
	await get_tree().create_timer(0.1).timeout # Prevent last input to be sent to player.
	cutscene_ended.emit()


func load_entities(level_name: String) -> void:
	var entities   := tile_map_by_coordinate_finder.find_entities_meta_for_level(get_tree().root, level_name)
	var level_node := tile_map_by_coordinate_finder.find_level_node(get_tree().root, level_name)
	for entity in entities:
		load_entity(entity, level_name, level_node)


func load_entity(entity: Dictionary, level_name: String, level_node: Node2D):
	print("load entity: " + entity.identifier)
	match entity.identifier:
		"OnPlayerEnter":
			create_on_player_enter_area(entity, level_name, level_node)
		"SpawnPlayer":
			pass


func create_on_player_enter_area(entity: Dictionary, level_name: String, level_node: Node2D):
	var area           := Area2D.new()
	var collisionShape := CollisionShape2D.new()
	var rectangleShape := RectangleShape2D.new()

	area.collision_layer = 1
	area.collision_mask = 1

	collisionShape.name = "CollisionShape2D"
	collisionShape.set_shape(rectangleShape)
	area.add_child(collisionShape)

	collisionShape.global_position = Vector2(
		level_node.global_position.x + entity.px.x + entity.width / 2,
		level_node.global_position.y + entity.px.y + entity.height /2)

	rectangleShape.size = Vector2(entity.width, entity.height)

	var trigger_name = entity.fields.Name
	var timeline_name = entity.fields.Timeline

	area.set_monitoring(true)

	if trigger_name or timeline_name:
		area.body_entered.connect(func (body): on_enter_area(body, level_name, trigger_name, timeline_name))

	get_tree().root.get_node("Main").add_child(area)

func on_enter_area(body, level_name: String, trigger_name, timeline_name):
	if body.is_in_group("Player"):
		if trigger_name:
			var level_controller = get_node(level_name)
			if (level_controller and level_controller.has_method("on_player_enter")):
				level_controller.on_player_enter(trigger_name)
		if timeline_name:
			start_timeline(timeline_name)

