class_name OverworldController

enum {
	DISABLED,
	IDLE
}

enum PlayerDirection {
	LEFT,
	RIGHT
}

const SPEED: float                 = 80.0

var state                         := IDLE
var player_direction              := PlayerDirection.RIGHT

var player: CharacterBody2D
var animated_sprite: AnimatedSprite2D

func _init(player_: CharacterBody2D, animated_sprite_: AnimatedSprite2D):
	player = player_
	animated_sprite = animated_sprite_

func physics_process(_delta):
	var direction = Input.get_vector("move_left", "move_right", "move_up", "move_down")

	match state:
		DISABLED:
			pass
		IDLE:
			if direction.length() > 0.0:
				animated_sprite.play("walk")
			else:
				animated_sprite.play("idle")

			if direction.length() > 0.0:
				player.velocity = lerp(player.velocity, direction * SPEED, 0.3)
			else:
				# Slide when stopping
				player.velocity.x = move_toward(player.velocity.x, 0, SPEED)
				player.velocity.y = move_toward(player.velocity.y, 0, SPEED)

			player.move_and_slide()

	if player.velocity.x > 0:
		animated_sprite.flip_h = false
		if player_direction == PlayerDirection.LEFT:
			pass
			# player_turned.emit("right")
		player_direction = PlayerDirection.RIGHT
	elif player.velocity.x < 0:
		animated_sprite.flip_h = true
		if player_direction == PlayerDirection.RIGHT:
			pass
			# player_turned.emit("left")
		player_direction = PlayerDirection.LEFT


func enter_state(next_state):
	print("Enter state: " + state_to_string(next_state))
	match state:
		DISABLED:
			animated_sprite.play("idle")
		IDLE:
			animated_sprite.play("idle")

	state = next_state

func state_to_string(s) -> String:
	match s:
		IDLE:
			return "IDLE"
		DISABLED:
			return "DISABLED"
		_:
			return ""

func enable():
	enter_state(IDLE)

func disable():
	enter_state(DISABLED)
