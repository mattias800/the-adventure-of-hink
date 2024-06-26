extends Camera2D

const LdtkUtil = preload("res://src/utils/LdtkUtil.gd")

var release_falloff            := 35
var acceleration               := 10
var max_speed                  := 20
var velocity: Vector2          =  Vector2.ZERO
var look_ahead: Vector2        =  Vector2.ZERO
var look_ahead_target: Vector2 =  Vector2.ZERO
const VIEWPORT      := Vector2(320, 180)
const HALF_VIEWPORT := VIEWPORT / 2

var target

enum {
	IDLE,
	CONTROLLED,
	FOLLOWING_TARGET,
	FOLLOWING_PLAYER
}

var state


func _ready():
	set_anchor_mode(Camera2D.ANCHOR_MODE_FIXED_TOP_LEFT)
	state = FOLLOWING_PLAYER
	CameraManager.set_camera(self)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	if target:
		match state:
			FOLLOWING_TARGET:
				var focus := clamp_vec2_to_limits(target.get_global_transform().get_origin() - HALF_VIEWPORT)
				global_transform.origin = lerp(global_transform.origin, focus, 0.1)

			FOLLOWING_PLAYER:
				look_ahead = lerp(look_ahead, look_ahead_target, 0.005)
				var focus := clamp_vec2_to_limits(target.position - HALF_VIEWPORT + look_ahead)
				position = lerp(position, focus, 0.1)

			CONTROLLED:
				# var input_vector = Input.get_vector("camera_move_left", "camera_move_right", "camera_move_up", "camera_move_down")
				var target_vector := get_vector_from_center_to_player()
				calculate_velocity(delta, target_vector)
				update_global_position(delta)

	clamp_camera_to_limits()


func _physics_process(_delta):
	clamp_camera_to_limits()


func set_camera_target(t: Node2D):
	target = t
	look_ahead_target = Vector2(0, 0)

func jump_to_target() -> void:
	position = clamp_vec2_to_limits(target.position - HALF_VIEWPORT)

func _on_player_turned(direction):
	match direction:
		"right":
			look_ahead_target = Vector2(50, 0)
		"left":
			look_ahead_target = Vector2(-50, 0)
		"up":
			look_ahead_target = Vector2(0, -50)
		"down":
			look_ahead_target = Vector2(0, 50)


func get_vector_from_center_to_player() -> Vector2:
	var x: Vector2 =  target.get_global_transform_with_canvas().get_origin() - get_viewport_rect().get_center()
	return x.normalized()

func update_global_position(delta: float):
	global_position += lerp(
		velocity,
		Vector2.ZERO,
		pow(2, -32 * delta)
	)

func clamp_camera_to_limits():
	# Prevent camera position to outside of limits.
	# This ensures that background does not move because of limit and position being out of sync.
	# Camera limits must be set first.
	# var zoomed_viewport_size = get_viewport_to_zoom_scale()
	var zoomed_viewport_size := Vector2(320, 180)
	var left_limit           := get_limit(SIDE_LEFT)
	var right_limit          := get_limit(SIDE_RIGHT) - zoomed_viewport_size.x
	var top_limit            := get_limit(SIDE_TOP)
	var bottom_limit         := get_limit(SIDE_BOTTOM) - zoomed_viewport_size.y

	position.x = clamp(position.x, left_limit, right_limit)
	position.y = clamp(position.y, top_limit, bottom_limit)


func clamp_vec2_to_limits(val: Vector2) -> Vector2:
	# Camera limits must be set first.
	var zoomed_viewport_size := get_viewport_to_zoom_scale()

	var left_limit   := get_limit(SIDE_LEFT)
	var right_limit  := get_limit(SIDE_RIGHT) + zoomed_viewport_size.x
	var top_limit    := get_limit(SIDE_TOP)
	var bottom_limit := get_limit(SIDE_BOTTOM) + zoomed_viewport_size.y

	return Vector2(
		clamp(val.x, left_limit, right_limit),
		clamp(val.y, top_limit, bottom_limit)
	)


func get_viewport_to_zoom_scale() -> Vector2i:
	var zoom_vector          := get_zoom()
	var zoomed_viewport_size := Vector2i(
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


func get_tilemap_size(source_tilemap: TileMap) -> Vector2i:
	var tilemap_rect := source_tilemap.get_used_rect()
	return Vector2i(
		tilemap_rect.end.x - tilemap_rect.position.x,
		tilemap_rect.end.y - tilemap_rect.position.y
	)
