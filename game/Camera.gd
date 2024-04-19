extends Camera2D

@export var tilemap: TileMap

var release_falloff = 35
var acceleration = 10
var max_speed = 20
var velocity: Vector2 = Vector2.ZERO

const VIEWPORT = Vector2(320, 180)
const HALF_VIEWPORT = VIEWPORT / 2

@onready var player: CharacterBody2D = %Player

enum {
	IDLE,
	CONTROLLED,
	FOLLOWING_TARGET
}

var state

func _ready():
	set_anchor_mode(Camera2D.ANCHOR_MODE_FIXED_TOP_LEFT)
	connect_to_level("Level_1")
	state = FOLLOWING_TARGET

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	match state:
		FOLLOWING_TARGET:
			var focus = clamp_vec2_to_limits(player.get_global_transform().get_origin() - HALF_VIEWPORT)
			global_transform.origin = lerp(global_transform.origin, focus, 0.1)
			
		CONTROLLED:
			# var input_vector = Input.get_vector("camera_move_left", "camera_move_right", "camera_move_up", "camera_move_down")
			var target_vector = get_vector_from_center_to_player()
			calculate_velocity(delta, target_vector)
			update_global_position(delta)
			
	clamp_camera_to_limits()

func _physics_process(delta):
	clamp_camera_to_limits()

func get_vector_from_center_to_player() -> Vector2:
	var player = get_tree().root.get_node("Main").get_node("Player")
	var x : Vector2 = player.get_global_transform_with_canvas().get_origin() - get_viewport_rect().get_center()
	return x.normalized()
	
func connect_to_level(level_name: String):
	var level = get_tree().root.get_node("Main").get_node("Hink").get_node(level_name)
	if not level:
		printerr("Could not find level: ", level_name)
		return
		
	var level_tilemap = level.get_node(level_name + "-tilemap-8x8")
		
	if level_tilemap:
		print("Found TileMap")
		tilemap = level_tilemap
		print(tilemap.name)
		# set_zoom(get_camera_zoom_to_tilemap(tilemap))
		set_zoom(Vector2.ONE)
		apply_camera_limits(tilemap)
	else:
		print("Found no TileMap")
	
	
func apply_camera_limits(tilemap: TileMap):
	var tilemap_info = get_tilemap_info(tilemap)
	var level_size = Vector2i(tilemap_info.tile_size * tilemap_info.size)
	var levelOffsetX = tilemap.get_parent().position.x
	var levelOffsetY = tilemap.get_parent().position.y
	set_limit(SIDE_TOP, levelOffsetY)
	set_limit(SIDE_BOTTOM, levelOffsetY + level_size.y)
	set_limit(SIDE_LEFT, levelOffsetX)
	set_limit(SIDE_RIGHT, levelOffsetX + level_size.x)
	
	print("Top bpttom left right")
	print(limit_top)
	print(limit_bottom)
	print(limit_left)
	print(get_limit(SIDE_LEFT))
	print(limit_right)
	
func update_global_position(delta: float):
	global_position += lerp(
		velocity,
		Vector2.ZERO,
		pow(2, -32 * delta)
	)
	
func clamp_camera_to_limits():
	# Camera limits must be set first.
	var zoomed_viewport_size = get_viewport_to_zoom_scale()
	
	var left_limit = get_limit(SIDE_LEFT)
	var right_limit = get_limit(SIDE_RIGHT) + zoomed_viewport_size.x
	var top_limit = get_limit(SIDE_TOP)
	var bottom_limit = get_limit(SIDE_BOTTOM) + zoomed_viewport_size.y
	
	global_position.x = clamp(global_position.x, left_limit, right_limit)
	global_position.y = clamp(global_position.y, top_limit, bottom_limit)

func clamp_vec2_to_limits(val: Vector2) -> Vector2:
	# Camera limits must be set first.
	var zoomed_viewport_size = get_viewport_to_zoom_scale()
	
	var left_limit = get_limit(SIDE_LEFT)
	var right_limit = get_limit(SIDE_RIGHT) + zoomed_viewport_size.x
	var top_limit = get_limit(SIDE_TOP)
	var bottom_limit = get_limit(SIDE_BOTTOM) + zoomed_viewport_size.y
	
	return Vector2(
		clamp(val.x, left_limit, right_limit),
		clamp(val.y, top_limit, bottom_limit)
	)
	
func get_viewport_to_zoom_scale():
	var zoom_vector = get_zoom()
	var zoomed_viewport_size = Vector2i(
		get_viewport().size[0] / zoom_vector.x,
		get_viewport().size[1] / zoom_vector.y,
	)
	
	return zoomed_viewport_size
	
func calculate_velocity(delta: float, direction: Vector2):
	velocity += direction * acceleration * delta
	
	if direction.x == 0:
		velocity.x = lerp(0.0, velocity.x, pow(2, -release_falloff * delta))
	if direction.y == 0:
		velocity.y = lerp(0.0, velocity.y, pow(2, -release_falloff * delta))
		
	velocity.x = clamp(
		velocity.x,
		-max_speed,
		max_speed
	)
	
	velocity.y = clamp(
		velocity.y,
		-max_speed,
		max_speed
	)

func get_camera_zoom_to_tilemap(tilemap: TileMap):
	var viewport_size = get_viewport().size # [x, y]
	
	var tilemap_info = get_tilemap_info(tilemap)
	var level_size = Vector2i(tilemap_info.tile_size * tilemap_info.size)
	
	var viewport_aspect = float(viewport_size[0]) / viewport_size[1]
	var level_aspect = float(level_size.x) / level_size.y
	
	var new_zoom = 1.0
	
	if level_aspect > viewport_aspect:
		new_zoom = float(viewport_size[1]) / level_size.y
	else:
		new_zoom = float(viewport_size[0]) / level_size.x
	
	new_zoom = clamp(new_zoom, 3.4, 3.6)
	
	return Vector2(new_zoom, new_zoom)
	
func get_tilemap_info(tilemap: TileMap):
	var tile_size = tilemap.get_tileset().tile_size
	
	var tilemap_rect = tilemap.get_used_rect()
	var tilemap_size = Vector2i(
		tilemap_rect.end.x - tilemap_rect.position.x,
		tilemap_rect.end.y - tilemap_rect.position.y
	)
	
	return {"size": tilemap_size, "tile_size": tile_size}
