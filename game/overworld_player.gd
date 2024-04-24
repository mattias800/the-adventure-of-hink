extends CharacterBody2D

enum {
	INACTIVE,
	IDLE
}

enum PlayerDirection {
	LEFT,
	RIGHT
}

const SPEED: float                 = 80.0

var state                         := IDLE
var player_direction              := PlayerDirection.RIGHT

func _ready():
	pass
	
func _physics_process(_delta):
	var a := $AnimatedSprite2D
	var direction = Input.get_vector("walk_left", "walk_right", "walk_up", "walk_down")
	
	match state:
		INACTIVE:
			pass
		IDLE:
			if direction.length() > 0.0:
				a.play("walk")
			else:
				a.play("idle")

			if direction.length() > 0.0:
				velocity = lerp(velocity, direction * SPEED, 0.3)
			else:
				# Slide when stopping
				velocity.x = move_toward(velocity.x, 0, SPEED)
				velocity.y = move_toward(velocity.y, 0, SPEED)
				
			move_and_slide()
			
	if velocity.x > 0:
		a.flip_h = false
		if player_direction == PlayerDirection.LEFT:
			pass
			# player_turned.emit("right")
		player_direction = PlayerDirection.RIGHT
	elif velocity.x < 0:
		a.flip_h = true
		if player_direction == PlayerDirection.RIGHT:
			pass
			# player_turned.emit("left")
		player_direction = PlayerDirection.LEFT


func enter_state(next_state):
	print("Enter state: " + state_to_string(next_state))
	var a := $AnimatedSprite2D
	
	match state:
		INACTIVE:
			a.play("idle")
		IDLE:
			a.play("idle")
	
	state = next_state

func state_to_string(s) -> String:
	match s:
		IDLE:
			return "IDLE"
		INACTIVE:
			return "INACTIVE"
		_:
			return ""

