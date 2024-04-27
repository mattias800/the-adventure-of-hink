extends Node

const LdtkUtil = preload("res://src/LdtkUtil.gd")

signal player_left_tilemap
signal player_entered_tilemap

var active_tile_map: TileMap
var active_level_name: String

@onready var camera: Camera2D = %Camera
@onready var platform_player := %Player
@onready var overworld_player := %OverworldPlayer
@onready var cutscene_manager := %CutsceneManager

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
	var result := LdtkUtil.find_tilemap_by_world_coordinates(get_tree().root, player_world_position)
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

func load_entities(level_name: String) -> void:
	var entities   := LdtkUtil.find_entities_meta_for_level(get_tree().root, level_name)
	var level_node := LdtkUtil.find_level_node(get_tree().root, level_name)
	for entity in entities:
		load_entity(entity, level_name, level_node)


func load_entity(entity: Dictionary, level_name: String, level_node: Node2D):
	print("load entity: " + entity.identifier)
	match entity.identifier:
		"OnPlayerEnter":
			create_on_player_enter_entity(entity, level_name, level_node)
		"Portal":
			create_portal_entity(entity, level_name, level_node)
		"SpawnPlayer":
			pass


func create_on_player_enter_entity(entity: Dictionary, level_name: String, level_node: Node2D):
	var trigger_name = entity.fields.Name
	var timeline_name = entity.fields.Timeline
	if trigger_name or timeline_name:
		create_on_player_enter_area2d(entity, level_name, level_node, func (body): on_player_enter_entity(body, level_name, trigger_name, timeline_name))

func on_player_enter_entity(body, level_name: String, trigger_name, timeline_name):
	if body.is_in_group("Player"):
		if trigger_name:
			var level_controller = get_node(level_name)
			if (level_controller and level_controller.has_method("on_player_enter")):
				level_controller.on_player_enter(trigger_name)
		if timeline_name:
			cutscene_manager.start_timeline(timeline_name)


func create_portal_entity(entity: Dictionary, level_name: String, level_node: Node2D):
	var target = entity.fields["Target"]

	if target:
		create_on_player_enter_area2d(entity, level_name, level_node, func (body): on_player_enter_portal(body, target))

func on_player_enter_portal(body, target: Dictionary):
	if body.is_in_group("Player"):
		var target_level = LdtkUtil.find_level_node_by_level_iid(get_tree().root, target["levelIid"])
		var target_portal = LdtkUtil.find_entity_by_iid(get_tree().root, target["entityIid"])
		if target_portal:
			var arrival_offset = LdtkUtil.get_vector_from_direction(target_portal.fields["ArrivalPlacement"])
			var world_x = target_portal.px[0]
			var world_y = target_portal.px[1]
			var next_position_for_player = target_level.global_position + Vector2(world_x, world_y) + arrival_offset
			teleport_player_to_level(target_level.name, next_position_for_player)
		else:
			push_error("Found no portal entity.")

# Create Area2D which triggers when player enters.
func create_on_player_enter_area2d(entity: Dictionary, level_name: String, level_node: Node2D, on_player_enter):
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

	area.set_monitoring(true)

	area.body_entered.connect(on_player_enter)

	get_tree().root.get_node("Main").add_child(area)

func teleport_player_to_level(level_name: String, next_global_position_for_player: Vector2):
	print("teleport_player_to_level")
	var level_node = LdtkUtil.find_level_node(get_tree().root, level_name)
	var settings = LdtkUtil.get_level_settings(level_node)
	print(level_node)
	print(settings)
	camera.connect_to_platform_level(level_name)
	if settings["LevelType"] == "Overworld":
		overworld_player.global_position = next_global_position_for_player
		set_current_player(overworld_player)
		camera.set_camera_target(overworld_player)
		overworld_player.enable()
		platform_player.disable()
	if settings["LevelType"] == "Platform":
		platform_player.global_position = next_global_position_for_player
		set_current_player(platform_player)
		camera.set_camera_target(platform_player)
		overworld_player.disable()
		platform_player.enable()

