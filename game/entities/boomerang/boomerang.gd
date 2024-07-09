extends CharacterBody2D

@onready var sprite_2d = $Sprite2D

signal collided_with_player

enum State {
	GOING_OUT,
	GOING_BACK,
	STUCK
}

var start_direction := Vector2(1, 0)
var current_direction := Vector2(1, 0)
var max_distance := 5.0
var max_speed := 200.0
var rotation_speed := 4.0
var rotating := true
var progress := 0.0
var going_back := false
var stuck := false

var state := State.GOING_OUT

var start_position := Vector2(0, 0)

# Called when the node enters the scene tree for the first time.
func _ready():
	throw(start_direction)

func throw(direction: Vector2):
	start_position = global_position
	start_direction = direction
	current_direction = direction
	progress = 0.0
	enter_state(State.GOING_OUT)

func _process(delta):
	match state:
		State.GOING_OUT:
			sprite_2d.rotate(PI * rotation_speed * delta)

		State.GOING_BACK:
			sprite_2d.rotate(PI * rotation_speed * delta)

		State.STUCK:
			pass
			
func enter_state(next: State):
	match next:
		State.GOING_OUT:
			pass

		State.GOING_BACK:
			pass

		State.STUCK:
			pass
	state = next
	
func _physics_process(delta):
	match state:
		State.GOING_OUT:
			progress += delta
			var current_speed = (1.0 - progress) * max_speed
			var collision = move_and_collide(current_direction * current_speed * delta)
			if progress > 1.0:
				enter_state(State.GOING_BACK)
			if collision != null:
				if collision.get_collider().is_in_group("player"):
					collided_with_player.emit()
					queue_free()
				else:
					enter_state(State.STUCK)

		State.GOING_BACK:
			progress += delta
			var current_speed = (1.0 - progress) * max_speed
			current_direction = global_position - GameManager.player.global_position
			var collision = move_and_collide(current_direction.normalized() * current_speed * delta)
			if collision != null:
				if collision.get_collider().is_in_group("player"):
					collided_with_player.emit()
					queue_free()
				else:
					enter_state(State.STUCK)

		State.STUCK:
			pass
	
