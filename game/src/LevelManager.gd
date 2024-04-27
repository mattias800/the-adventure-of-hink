extends Node

const LdtkUtil = preload("res://src/utils/LdtkUtil.gd")

signal player_left_tilemap
signal player_entered_tilemap

var active_tile_map: TileMap
var active_level_node: Node2D

@onready var cutscene_manager := %CutsceneManager
@onready var game_manager := %GameManager

enum LevelType {
	PLATFORM,
	OVERWORLD
}

func check_for_level_change(player_world_position: Vector2):
	var result := LdtkUtil.find_tilemap_by_world_coordinates(get_tree().root, player_world_position)
	match result:
		[true, var tilemap, var level_node]:
			if (tilemap != active_tile_map):
				if (active_tile_map):
					player_leave_map(active_level_node, active_tile_map)
				active_tile_map = tilemap
				active_level_node = level_node
				player_enter_map(active_level_node, active_tile_map)

		[false, _, _]:
			print("Could not find tilemap containing player.")


func player_enter_map(level_node: Node2D, tilemap: TileMap):
	print("player_enter_map: ", level_node.name, tilemap.name)
	load_entities(level_node)
	var controller_name: String = level_node.name
	var level_controller = get_node(controller_name)
	if level_controller and level_controller.has_method("on_player_enter_map"):
		level_controller.on_player_enter_map()

	player_entered_tilemap.emit(level_node, tilemap, get_level_metadata(level_node))

func get_level_metadata(level_node: Node2D):
	return level_node.get_meta("LDtk_level_fields")

func player_leave_map(level_node: Node2D, tilemap: TileMap):
	print("player_leave_map", level_node.n)
	# unload_entities(level_name)
	var controller_name: String = level_node.name
	var level_controller = get_node(controller_name)
	if level_controller and level_controller.has_method("on_player_leave_map"):
		level_controller.on_player_leave_map()

	player_left_tilemap.emit(level_node, tilemap, get_level_metadata(level_node))

func load_entities(level_node: Node2D) -> void:
	var entities   := LdtkUtil.find_entities_meta_for_level(level_node)
	for entity in entities:
		load_entity(entity, level_node)


func load_entity(entity: Dictionary, level_node: Node2D):
	print("load entity: " + entity.identifier)
	match entity.identifier:
		"OnPlayerEnter":
			create_on_player_enter_entity(entity, level_node)
		"Portal":
			create_portal_entity(entity, level_node)
		"SpawnPlayer":
			pass


func create_on_player_enter_entity(entity: Dictionary, level_node: Node2D):
	var trigger_name = entity.fields.Name
	var timeline_name = entity.fields.Timeline
	if trigger_name or timeline_name:
		create_on_player_enter_area2d(entity, level_node, func (body): on_player_enter_entity(body, level_node, trigger_name, timeline_name))

func on_player_enter_entity(body, level_node: Node2D, trigger_name, timeline_name):
	if body.is_in_group("Player"):
		# TODO NEXT STEP
		# Use level_node instead of level_name for all these methods.
		if trigger_name:
			var controller_name: String = level_node.name
			var level_controller = get_node(controller_name)
			if (level_controller and level_controller.has_method("on_player_enter")):
				level_controller.on_player_enter(trigger_name)
		if timeline_name:
			cutscene_manager.start_timeline(timeline_name)


func create_portal_entity(entity: Dictionary, level_node: Node2D):
	var target = entity.fields["Target"]

	if target:
		create_on_player_enter_area2d(entity, level_node, func (body): on_player_enter_portal(body, target))

func on_player_enter_portal(body, target: Dictionary):
	if body.is_in_group("Player"):
		var target_level = LdtkUtil.find_level_node_by_level_iid(get_tree().root, target["levelIid"])
		var target_portal = LdtkUtil.find_entity_by_iid(get_tree().root, target["entityIid"])
		if target_portal:
			var arrival_offset = LdtkUtil.get_vector_from_direction(target_portal.fields["ArrivalPlacement"])
			var world_x = target_portal.px[0]
			var world_y = target_portal.px[1]
			var next_position_for_player = target_level.global_position + Vector2(world_x, world_y) + arrival_offset
			game_manager.teleport_player_to_level(target_level.name, next_position_for_player)
		else:
			push_error("Found no portal entity.")

# Create Area2D which triggers when player enters.
func create_on_player_enter_area2d(entity: Dictionary, level_node: Node2D, on_player_enter):
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


