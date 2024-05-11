class_name PlatformController

signal player_turned
signal player_started_moving_on_ground
signal player_stopped_moving_on_ground
signal player_dash_started(direction: Vector2)
signal player_dash_stopped

const SPEED: float                 = 80.0
const DASH_SPEED: float            = 400.0
const DASH_TIME: float             = 0.1
const MAX_FALL_SPEED: float        = 250.0
const JUMP_HORIZONTAL_SPEED: float = 7.0
const MAX_HORIZONTAL_SPEED: float  = 80.0
const JUMP_VELOCITY: float         = 150.0
const JUMP_RELEASE_VELOCITY: float = 100.0
const WALL_GRAB_TIME_LIMIT: float  = 5.0
const WALL_CLIMB_SPEED: float  = 40.0
const COYOTE_TIME_LIMIT: float     = 0.05

# Get the gravity from the project settings to be synced with RigidBody nodes.
var gravity = ProjectSettings.get_setting("physics/2d/default_gravity")
# var gravity: float = 600.0

enum JumpSource {
	GROUND,
	WALL,
	AIR
}
enum {
	IDLE,
	JUMPING,
	FALLING,
	GRABBING_WALL,
	WALL_SLIDING,
	DASHING,
	DISABLE_INPUT,
	DISABLED
}
enum PlayerDirection {
	LEFT,
	RIGHT
}
var state                         := IDLE
var player_direction              := PlayerDirection.RIGHT
var time_since_no_ground          := 0.0
var time_until_wall_grab_possible := 0.0
var wall_grab_time_left           := 0.0
var dash_time_left                := 0.0
var dash_direction                := Vector2.ZERO
var time_until_jump_velocity_reset_allowed := 0.0
var time_until_jump_horizontal_control := 0.0
var jumps_left                    := 0
var dashes_left                    := 0
var num_double_jumps              := 1
var num_dashes              := 1
var is_against_wall := false
var coyote_time_left              := 0.0
var velocity_into_wall            := 0.0 # The character velocity when hitting the wall. Need to reuse when calculating is_on_wall()
var active_tile_map: TileMap      =  null # Reference to active TileMap, used to check if player has entered new TileMap
var player: CharacterBody2D
var animated_sprite: AnimatedSprite2D
var jump_sound: AudioStreamPlayer2D
var land_sound: AudioStreamPlayer2D
var dash_sound: AudioStreamPlayer2D
var grab_wall_sound: AudioStreamPlayer2D
var jump_from_wall_sound: AudioStreamPlayer2D
var jump_from_air_sound: AudioStreamPlayer2D
var ray_cast_left: RayCast2D
var ray_cast_right: RayCast2D

func _init(\
		player_: CharacterBody2D, \
		animated_sprite_: AnimatedSprite2D, \
		jump_sound_: AudioStreamPlayer2D, \
		land_sound_: AudioStreamPlayer2D, \
		dash_sound_: AudioStreamPlayer2D, \
		grab_wall_sound_: AudioStreamPlayer2D, \
		jump_from_wall_sound_: AudioStreamPlayer2D, \
		jump_from_air_sound_: AudioStreamPlayer2D):
	player = player_
	animated_sprite = animated_sprite_
	jump_sound = jump_sound_
	land_sound = land_sound_
	dash_sound = dash_sound_
	grab_wall_sound = grab_wall_sound_
	jump_from_wall_sound = jump_from_wall_sound_
	jump_from_air_sound = jump_from_air_sound_
	ray_cast_left = player.ray_cast_left
	ray_cast_right = player.ray_cast_right


func physics_process(delta):
	var direction := Input.get_axis("move_left", "move_right")
	var vertical_direction := Input.get_axis("move_up", "move_down")

	match state:
		DISABLED:
			return
		DISABLE_INPUT:
			return
		GRABBING_WALL:
			dashes_left = num_dashes
			wall_grab_time_left -= delta
			if vertical_direction == 0:
				animated_sprite.play("grabbing_wall")

			if wall_grab_time_left <= 0.0:
				enter_state(WALL_SLIDING)
			elif Input.is_action_just_pressed("jump"):
				trigger_jump(JumpSource.WALL)
				var jump_direction := Vector2(player.get_wall_normal().x, -1)
				player.velocity = jump_direction.normalized() * JUMP_VELOCITY
				player.move_and_slide()
			elif Input.is_action_pressed("move_up"):
				if player.is_on_ceiling():
					animated_sprite.play("grabbing_wall")
				elif not Input.is_action_just_pressed("move_up"):
					animated_sprite.play("climbing")
				player.velocity.x = velocity_into_wall * JUMP_VELOCITY
				player.velocity.y = vertical_direction * WALL_CLIMB_SPEED
				player.move_and_slide()
				if not player.is_on_wall():
					enter_state(IDLE)
			elif Input.is_action_pressed("move_down"):
				animated_sprite.play("climbing")
				player.velocity.x = velocity_into_wall * JUMP_VELOCITY
				player.velocity.y = vertical_direction * WALL_CLIMB_SPEED
				player.move_and_slide()
				if not player.is_on_wall():
					enter_state(IDLE)
			elif (player.get_wall_normal().x * direction) > 0:
				# User pressed away from wall
				coyote_time_left = COYOTE_TIME_LIMIT
				enter_state(FALLING)
			elif not Input.is_action_pressed("grab_wall"):
				# User relased wall grab button
				coyote_time_left = COYOTE_TIME_LIMIT
				if (player.get_wall_normal().x * direction) < 0:
					# User is pressing against wall
					enter_state(WALL_SLIDING)
				else:
					enter_state(FALLING)
			else:
				# Multiply with high value to ensure it doesn't lose connection with moving platforms.
				player.velocity.x = velocity_into_wall * JUMP_VELOCITY
				player.velocity.y = 0

				player.move_and_slide()

				if not player.is_on_wall():
					enter_state(FALLING)

		WALL_SLIDING:
			player.velocity.y = lerp(player.velocity.y, 50.0, 0.1)
			player.velocity.x = direction

			player.move_and_slide()

			if player.is_on_wall():
				if (player.get_wall_normal().x * direction) > 0:
					# User pressed away from wall
					coyote_time_left = COYOTE_TIME_LIMIT
					enter_state(FALLING)
				if direction == 0:
					# User released hold towards wall
					coyote_time_left = COYOTE_TIME_LIMIT
					player.velocity.x = 0
					enter_state(FALLING)

				if Input.is_action_just_pressed("jump"):
					trigger_jump(JumpSource.WALL)
					var jump_direction := Vector2(player.get_wall_normal().x, -1)
					player.velocity = jump_direction.normalized() * JUMP_VELOCITY

				if Input.is_action_just_pressed("grab_wall") and wall_grab_time_left > 0:
					enter_state(GRABBING_WALL)
				if player.is_on_floor():
					enter_state(IDLE)
			else:
				coyote_time_left = COYOTE_TIME_LIMIT
				enter_state(FALLING)


		JUMPING:
			time_until_jump_velocity_reset_allowed -= delta
			time_until_jump_horizontal_control -= delta
			player.velocity.y += gravity * delta
			time_since_no_ground += delta
			time_until_wall_grab_possible = time_until_wall_grab_possible - delta

			# Dampen jump if jump button is released
			if not Input.is_action_pressed("jump") and player.velocity.y < JUMP_RELEASE_VELOCITY and time_until_jump_velocity_reset_allowed <= 0:
				player.velocity.y = -50
				enter_state(FALLING)
			elif Input.is_action_just_pressed("jump") and jumps_left > 0:
				trigger_jump(JumpSource.AIR)
				player.velocity.y = -JUMP_VELOCITY
			elif Input.is_action_just_pressed("dash") and dashes_left > 0:
				trigger_dash()
			else:
				if time_until_jump_horizontal_control <= 0:
					add_velocity_x(direction * JUMP_HORIZONTAL_SPEED)

				player.move_and_slide()

				if player.velocity.y > 0:
					enter_state(FALLING)

				if player.is_on_floor():
					enter_state(IDLE)

				if player.is_on_wall() and time_until_wall_grab_possible <= 0.0 and Input.is_action_pressed("grab_wall"):
					velocity_into_wall = player.get_wall_normal().x * -1
					if wall_grab_time_left >= 0.0:
						enter_state(GRABBING_WALL)
					else:
						enter_state(WALL_SLIDING)

		FALLING:
			time_until_jump_velocity_reset_allowed -= delta
			time_until_jump_horizontal_control -= delta

			if coyote_time_left >= 0.0:
				coyote_time_left -= delta

			time_since_no_ground += delta
			time_until_wall_grab_possible = time_until_wall_grab_possible - delta
			player.velocity.y = minf(player.velocity.y + gravity * delta, MAX_FALL_SPEED)

			if Input.is_action_just_pressed("jump") and coyote_time_left > 0.0:
				trigger_jump(JumpSource.GROUND)
				player.velocity.y = -JUMP_VELOCITY
				player.velocity.x = direction * SPEED # TODO Always * SPEED here?
			elif Input.is_action_just_pressed("jump") and jumps_left > 0:
				trigger_jump(JumpSource.AIR)
				player.velocity.y = -JUMP_VELOCITY
			elif Input.is_action_just_pressed("dash") and dashes_left > 0:
				trigger_dash()
			else:
				if time_until_jump_horizontal_control <= 0:
					add_velocity_x(direction * JUMP_HORIZONTAL_SPEED)

				player.move_and_slide()

				if player.is_on_floor():
					land_sound.play()
					enter_state(IDLE)

				if player.is_on_wall():
					if Input.is_action_pressed("grab_wall") and time_until_wall_grab_possible <= 0.0:
						velocity_into_wall = player.get_wall_normal().x * -1
						if wall_grab_time_left >= 0.0:
							enter_state(GRABBING_WALL)
					elif direction != 0:
						enter_state(WALL_SLIDING)
		DASHING:
			player.velocity = dash_direction * DASH_SPEED
			player.move_and_slide()

			dash_time_left -= delta
			if dash_time_left <= 0:
				player.velocity *= 0.3
				player_dash_stopped.emit()
				enter_state(JUMPING)

		IDLE:
			if not player.is_on_floor():
				jumps_left = 0
				dashes_left = num_dashes
				coyote_time_left = COYOTE_TIME_LIMIT
				player_stopped_moving_on_ground.emit()
				enter_state(FALLING)
			else:
				var is_jump_allowed := player.is_on_floor() or time_since_no_ground < 0.1

				# Handle jump.
				if Input.is_action_just_pressed("jump") and is_jump_allowed:
					player_stopped_moving_on_ground.emit()
					trigger_jump(JumpSource.GROUND)
					player.velocity.y = -JUMP_VELOCITY
					player.velocity.x = direction * SPEED

				else:
					# Get the input direction and handle the movement/deceleration.
					# As good practice, you should replace UI actions with custom gameplay actions.

					if direction != 0.0:
						animated_sprite.play("walk")
						player_started_moving_on_ground.emit()
					else:
						animated_sprite.play("idle")
						player_stopped_moving_on_ground.emit()

					if direction:
						player.velocity.x = lerp(player.velocity.x, direction * SPEED, 0.3)
					else:
						# Slide when stopping
						player.velocity.x = move_toward(player.velocity.x, 0, SPEED)

					player.move_and_slide()

	if player.velocity.x > 0:
		animated_sprite.flip_h = false
		if player_direction == PlayerDirection.LEFT:
			player_turned.emit("right")
		player_direction = PlayerDirection.RIGHT
	elif player.velocity.x < 0:
		animated_sprite.flip_h = true
		if player_direction == PlayerDirection.RIGHT:
			player_turned.emit("left")
		player_direction = PlayerDirection.LEFT


func enter_state(next_state):
	print("Enter state: ", state_to_string(next_state))
	match next_state:
		DISABLE_INPUT:
			animated_sprite.play("idle")

		IDLE:
			wall_grab_time_left = WALL_GRAB_TIME_LIMIT
			animated_sprite.play("idle")

		JUMPING:
			animated_sprite.play("jump")

		FALLING:
			animated_sprite.play("fall")

		GRABBING_WALL:
			grab_wall_sound.play()
			animated_sprite.play("grabbing_wall")
			player.velocity.y = 0
			wall_grab_time_left = WALL_GRAB_TIME_LIMIT

		WALL_SLIDING:
			animated_sprite.play("grabbing_wall")

	state = next_state


func add_velocity_x(val: float):
	player.velocity.x = clamp(player.velocity.x + val, -MAX_HORIZONTAL_SPEED, MAX_HORIZONTAL_SPEED)


func state_to_string(s) -> String:
	match s:
		IDLE:
			return "IDLE"
		JUMPING:
			return "JUMPING"
		FALLING:
			return "FALLING"
		GRABBING_WALL:
			return "GRABBING_WALL"
		WALL_SLIDING:
			return "WALL_SLIDING"
		_:
			return ""


func trigger_jump(jump_source: JumpSource):
	match jump_source:
		JumpSource.GROUND:
			jump_sound.play()
			time_since_no_ground = 0.0
			time_until_wall_grab_possible = 0.1
			dashes_left = num_dashes
			jumps_left = num_double_jumps
			time_until_jump_velocity_reset_allowed = 0.0
			time_until_jump_horizontal_control = 0.0

		JumpSource.AIR:
			jump_from_air_sound.play()
			jumps_left -= 1
			time_until_jump_velocity_reset_allowed = 0.5
			time_until_jump_horizontal_control = 0.1

		JumpSource.WALL:
			jump_from_wall_sound.play()
			time_until_wall_grab_possible = 0.1
			dashes_left = num_dashes
			jumps_left = num_double_jumps
			time_until_jump_velocity_reset_allowed = 1.0
			time_until_jump_horizontal_control = 0.1


	enter_state(JUMPING)
	jump_sound.play()

func trigger_dash():
	dash_sound.play()
	dash_direction = Input.get_vector("move_left", "move_right", "move_up", "move_down")

	# If no direction is held by player, just dash forward
	if dash_direction == Vector2.ZERO:
		dash_direction = Vector2(1 if player_direction == PlayerDirection.RIGHT else -1, 0)

	dash_time_left = DASH_TIME
	dashes_left -= 1
	player_dash_started.emit(dash_direction)
	enter_state(DASHING)

func trigger_force(force: Vector2):
	print("trigger_force")
	print(force)
	player.velocity = force
	dashes_left = num_dashes
	jumps_left = num_double_jumps
	time_since_no_ground = 0.0
	wall_grab_time_left = WALL_GRAB_TIME_LIMIT
	time_until_jump_velocity_reset_allowed = 5
	enter_state(JUMPING)

func on_hit_jump_source():
	# TODO Handle user jump action here, including coyote time?
	player.velocity.y = -JUMP_VELOCITY
	trigger_jump(PlatformController.JumpSource.GROUND)
	time_until_jump_velocity_reset_allowed = 0.2

func enable():
	enter_state(IDLE)


func disable():
	enter_state(DISABLE_INPUT)

func is_node_is_wall_jumpable(node: Node2D):
	return ray_cast_right.get_collider().get_class() == "TileMap"
