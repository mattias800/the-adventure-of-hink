class_name PlatformController

signal player_turned

const SPEED: float                 = 80.0
const MAX_FALL_SPEED: float        = 250.0
const JUMP_HORIZONTAL_SPEED: float = 7.0
const MAX_HORIZONTAL_SPEED: float  = 80.0
const JUMP_VELOCITY: float         = 150.0
const JUMP_RELEASE_VELOCITY: float = 100.0
const WALL_GRAB_TIME_LIMIT: float  = 1.0
const COYOTE_TIME_LIMIT: float = 0.05

# Get the gravity from the project settings to be synced with RigidBody nodes.
# var gravity = ProjectSettings.get_setting("physics/2d/default_gravity")
var gravity: float = 600.0

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
var jumps_left                    := 0
var num_double_jumps              := 1
var coyote_time_left              := 0.0
var velocity_into_wall            := 0.0 # The character velocity when hitting the wall. Need to reuse when calculating is_on_wall()
var active_tile_map: TileMap      =  null # Reference to active TileMap, used to check if player has entered new TileMap

var player: CharacterBody2D
var animated_sprite: AnimatedSprite2D
var jump_sound: AudioStreamPlayer2D

func _init(player_: CharacterBody2D, animated_sprite_: AnimatedSprite2D, jump_sound_: AudioStreamPlayer2D):
	player = player_
	animated_sprite = animated_sprite_
	jump_sound = jump_sound_

func physics_process(delta):
	var direction := Input.get_axis("walk_left", "walk_right")

	match state:
		DISABLED:
			return
		DISABLE_INPUT:
			return
		GRABBING_WALL:
			wall_grab_time_left -= delta
			if wall_grab_time_left <= 0.0:
				enter_state(WALL_SLIDING)
			elif Input.is_action_just_pressed("jump"):
				trigger_jump(JumpSource.WALL)
				var jump_direction := Vector2(player.get_wall_normal().x, -1)
				player.velocity = jump_direction.normalized() * JUMP_VELOCITY

			if (player.get_wall_normal().x * direction) > 0:
				# User pressed away from wall
				coyote_time_left = COYOTE_TIME_LIMIT
				enter_state(FALLING)

			player.move_and_slide()

		WALL_SLIDING:
			player.velocity.y = lerp(player.velocity.y, 50.0, 0.1)
			player.velocity.x = velocity_into_wall

			player.move_and_slide()

			if player.is_on_wall():
				if (player.get_wall_normal().x * direction) > 0:
					# User pressed away from wall
					coyote_time_left = COYOTE_TIME_LIMIT
					enter_state(FALLING)

				if Input.is_action_just_pressed("jump"):
					trigger_jump(JumpSource.WALL)
					var jump_direction := Vector2(player.get_wall_normal().x, -1)
					player.velocity = jump_direction.normalized() * JUMP_VELOCITY

				if player.is_on_floor():
					enter_state(IDLE)
			else:
				coyote_time_left = COYOTE_TIME_LIMIT
				enter_state(FALLING)


		JUMPING:
			player.velocity.y += gravity * delta
			time_since_no_ground += delta
			time_until_wall_grab_possible = time_until_wall_grab_possible - delta

			# Dampen jump if jump button is released
			if not Input.is_action_pressed("jump") and player.velocity.y < JUMP_RELEASE_VELOCITY:
				player.velocity.y = -50
				enter_state(FALLING)

			if Input.is_action_just_pressed("jump") and jumps_left > 0:
				trigger_jump(JumpSource.AIR)
				player.velocity.y = -JUMP_VELOCITY

			add_velocity_x(direction * JUMP_HORIZONTAL_SPEED)

			player.move_and_slide()

			if player.velocity.y > 0:
				enter_state(FALLING)

			if player.is_on_floor():
				enter_state(IDLE)

			if player.is_on_wall() and time_until_wall_grab_possible <= 0.0:
				velocity_into_wall = player.get_wall_normal().x * -1
				if wall_grab_time_left >= 0.0:
					enter_state(GRABBING_WALL)
				else:
					enter_state(WALL_SLIDING)

		FALLING:
			if coyote_time_left >= 0.0:
				coyote_time_left -= delta

			time_since_no_ground += delta
			time_until_wall_grab_possible = time_until_wall_grab_possible - delta
			player.velocity.y = minf(player.velocity.y + gravity * delta, MAX_FALL_SPEED)

			if Input.is_action_just_pressed("jump") and coyote_time_left > 0.0:
				trigger_jump(JumpSource.AIR)
				player.velocity.y = -JUMP_VELOCITY
				player.velocity.x = direction * SPEED # TODO Always * SPEED here?

			if Input.is_action_just_pressed("jump") and jumps_left > 0:
				jumps_left -= 1
				player.velocity.y = -JUMP_VELOCITY
				jump_sound.play()

			add_velocity_x(direction * JUMP_HORIZONTAL_SPEED)

			player.move_and_slide()

			if player.is_on_floor():
				enter_state(IDLE)

			if player.is_on_wall() and time_until_wall_grab_possible <= 0.0:
				velocity_into_wall = player.get_wall_normal().x * -1
				if wall_grab_time_left >= 0.0:
					enter_state(GRABBING_WALL)
				else:
					enter_state(WALL_SLIDING)

		IDLE:
			if not player.is_on_floor():
				jumps_left = 0
				coyote_time_left = COYOTE_TIME_LIMIT
				enter_state(FALLING)
			else:
				var is_jump_allowed := player.is_on_floor() or time_since_no_ground < 0.1

				# Handle jump.
				if Input.is_action_just_pressed("jump") and is_jump_allowed:
					trigger_jump(JumpSource.GROUND)
					player.velocity.y = -JUMP_VELOCITY
					player.velocity.x = direction * SPEED

				else:
					# Get the input direction and handle the movement/deceleration.
					# As good practice, you should replace UI actions with custom gameplay actions.

					if direction != 0.0:
						animated_sprite.play("walk")
					else:
						animated_sprite.play("idle")

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
			time_since_no_ground = 0.0
			time_until_wall_grab_possible = 0.1
			jumps_left = num_double_jumps

		JumpSource.AIR:
			jumps_left -= 1

		JumpSource.WALL:
			time_until_wall_grab_possible = 0.1
			jumps_left = num_double_jumps

	enter_state(JUMPING)
	jump_sound.play()

func enable():
	enter_state(IDLE)
	
func disable():
	enter_state(DISABLE_INPUT)
