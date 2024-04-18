extends CharacterBody2D


const SPEED = 80.0
const JUMP_HORIZONTAL_SPEED = 7.0
const MAX_HORIZONTAL_SPEED = 80.0

const JUMP_VELOCITY = 250.0
const JUMP_RELEASE_VELOCITY = 100.0

# Get the gravity from the project settings to be synced with RigidBody nodes.
# var gravity = ProjectSettings.get_setting("physics/2d/default_gravity")
var gravity = 600

enum {
	IDLE,
	JUMPING,
	FALLING,
	GRABBING_WALL,
	WALL_SLIDING
}

var state := IDLE
var time_since_no_ground := 0.0
var time_since_wall_grab := 0.0
var time_until_wall_grab_possible := 0.0

var jumps_left := 0
var num_double_jumps := 1

func _physics_process(delta):
	var a := $AnimatedSprite2D
	var direction := Input.get_axis("ui_left", "ui_right")

	match state:
		
		GRABBING_WALL:
			time_since_wall_grab = time_since_wall_grab + delta
			
			if time_since_wall_grab > 1.0:
				start_state(WALL_SLIDING)	
			elif Input.is_action_just_pressed("ui_accept"):
				start_state(JUMPING)
				jumps_left = num_double_jumps
				time_until_wall_grab_possible = 0.1
				var jump_direction = Vector2(get_wall_normal().x, -1)
				velocity = jump_direction.normalized() * JUMP_VELOCITY
			
			if get_wall_normal().x * direction > 0:
				# User pressed away from wall
				start_state(FALLING)
				
			move_and_slide()
		
		WALL_SLIDING:
			velocity.y = lerp(velocity.y, 50.0, 0.2)

			if not is_on_wall():
				start_state(FALLING)
			
			if Input.is_action_just_pressed("ui_accept"):
				start_state(IDLE)
				jumps_left = num_double_jumps
				velocity.y = -JUMP_VELOCITY
				time_until_wall_grab_possible = 0.1
				velocity.x = get_wall_normal().x * JUMP_VELOCITY
				
			if is_on_floor():
				start_state(IDLE)
				
			move_and_slide()
		
		JUMPING:
			velocity.y += gravity * delta
			time_since_no_ground += delta
			time_until_wall_grab_possible = time_until_wall_grab_possible - delta

			# Dampen jump if jump button is released
			if not Input.is_action_pressed("ui_accept") and velocity.y < JUMP_RELEASE_VELOCITY:
				velocity.y = -50
				start_state(FALLING)

			if Input.is_action_just_pressed("ui_accept") and jumps_left > 0:
				jumps_left -= 1
				velocity.y = -JUMP_VELOCITY
				
			add_velocity_x(direction * JUMP_HORIZONTAL_SPEED)

			move_and_slide()

			if velocity.y > 0:
				start_state(FALLING)
				
			if is_on_floor():
				start_state(IDLE)
				if is_on_wall() and time_until_wall_grab_possible:
					start_state(GRABBING_WALL)
					
		FALLING:
			time_since_no_ground += delta
			time_until_wall_grab_possible = time_until_wall_grab_possible - delta
			velocity.y += gravity * delta

			if Input.is_action_just_pressed("ui_accept") and jumps_left > 0:
				jumps_left -= 1
				velocity.y = -JUMP_VELOCITY

			add_velocity_x(direction * JUMP_HORIZONTAL_SPEED)
				
			move_and_slide()

			if is_on_floor():
				start_state(IDLE)
			
			if is_on_wall() and time_until_wall_grab_possible <= 0.0:
				start_state(GRABBING_WALL)
				
		IDLE:			
			if not is_on_floor():
				jumps_left = 0
				start_state(FALLING)
			else:
				var is_jump_allowed = is_on_floor() or time_since_no_ground < 0.1
				
				# Handle jump.
				if Input.is_action_just_pressed("ui_accept") and is_jump_allowed:
					time_since_no_ground = 0.0
					jumps_left = num_double_jumps
					velocity.y = -JUMP_VELOCITY
					velocity.x = direction * SPEED
					start_state(JUMPING)
				else:
					# Get the input direction and handle the movement/deceleration.
					# As good practice, you should replace UI actions with custom gameplay actions.
					
					if direction != 0.0:
						a.play("walk")
					else:
						a.play("idle")
						
					if direction:
						velocity.x = lerp(velocity.x, direction * SPEED, 0.3)
					else:
						# Slide when stopping
						velocity.x = move_toward(velocity.x, 0, SPEED)

					move_and_slide()
	
	if velocity.x > 0:
		a.flip_h = false
	elif velocity.x < 0:
		a.flip_h = true

func start_state(next_state):
	print("Start state: ", state_to_string(next_state))
	var a := $AnimatedSprite2D

	match next_state:
		IDLE:
			a.play("idle")
			
		JUMPING:
			a.play("jump")
			
		FALLING:
			a.play("fall")
			
		GRABBING_WALL:
			a.play("grabbing_wall")
			velocity.x = 0
			velocity.y = 0
			time_since_wall_grab = 0.0
			
		WALL_SLIDING:
			velocity.x = 0

	print(a.animation)
	state = next_state

func add_velocity_x(val: float):
	velocity.x = clamp(velocity.x + val, -MAX_HORIZONTAL_SPEED, MAX_HORIZONTAL_SPEED)
		
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
