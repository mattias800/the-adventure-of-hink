extends GPUParticles2D

const GRAVITY_STRENGTH = 0

func _ready():
	emitting = false
	amount_ratio = 0.5
	position = Vector2i(0, 4)

func turn_left():
	#position = Vector2i(2, 4)
	process_material.gravity = Vector3(GRAVITY_STRENGTH, 0, 0)

func turn_right():
	#position = Vector2i(-2, 4)
	process_material.gravity = Vector3(-GRAVITY_STRENGTH, 0, 0)


func _on_player_player_turned(direction: String):
	match direction:
		"left":
			turn_left()
		"right":
			turn_right()


func _on_player_player_started_moving_on_ground():
	emitting = true
	amount_ratio = 0.5
	amount = 8


func _on_player_player_stopped_moving_on_ground():
	emitting = false


func _on_player_player_dash_started(_direction):
	emitting = true
	amount_ratio = 1
	amount = 16


func _on_player_player_dash_stopped():
	emitting = false
