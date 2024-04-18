extends CharacterBody2D


const SPEED = 100.0
const JUMP_VELOCITY = 400.0
const JUMP_RELEASE_VELOCITY = 100.0

# Get the gravity from the project settings to be synced with RigidBody nodes.
var gravity = ProjectSettings.get_setting("physics/2d/default_gravity")

var time_since_no_ground = 0.0
var jumps_left = 0

func _physics_process(delta):
	# Add the gravity.
	var current_velocity_y = -velocity.y
	
	if is_on_floor():
		time_since_no_ground = 0.0
		jumps_left = 2
	else:
		current_velocity_y -= gravity * delta
		time_since_no_ground += delta

	var is_jump_allowed = is_on_floor() or time_since_no_ground < 0.1 or jumps_left > 0
	
	# Handle jump.
	if Input.is_action_just_pressed("ui_accept") and is_jump_allowed:
		jumps_left = jumps_left - 1
		print("jumps_left", jumps_left)
		current_velocity_y = JUMP_VELOCITY

	if not Input.is_action_pressed("ui_accept") and current_velocity_y > JUMP_RELEASE_VELOCITY:
		current_velocity_y = JUMP_RELEASE_VELOCITY
		
	# Get the input direction and handle the movement/deceleration.
	# As good practice, you should replace UI actions with custom gameplay actions.
	var direction = Input.get_axis("ui_left", "ui_right")
	
	if direction:
		velocity.x = direction * SPEED
	else:
		velocity.x = move_toward(velocity.x, 0, SPEED)

	velocity.y = -current_velocity_y
	move_and_slide()
