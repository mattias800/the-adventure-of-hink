extends Node2D
class_name Boomerang

@onready var area_2d = $Area2D
@onready var sprite_2d = $Sprite2D
@onready var start_sound = $StartSound
@onready var stuck_sound = $StuckSound
@onready var flying_sound = $FlyingSound

signal collided_with_player()
signal hit_body(body: Node2D)

enum State {
	GOING_OUT,
	GOING_BACK,
	STUCK
}

# Movement
var current_velocity := Vector2(1, 0)
var max_speed := 200.0

var outgoing_progress := 0.0
var outgoing_threshold = 0.2
var angular_velocity_multiplier := 0.07

# Visual
var rotation_speed := 4.0
var rotating := true

var state := State.GOING_OUT

func throw(direction: Vector2):
	current_velocity = direction
	outgoing_progress = 0.0
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
	print("Boomerang state: " + State.keys()[next])
	match next:
		State.GOING_OUT:
			start_sound.play(0.05)
			flying_sound.play()

		State.GOING_BACK:
			pass

		State.STUCK:
			stuck_sound.play()
			flying_sound.stop()
	state = next
	
func _physics_process(delta):
	match state:
		State.GOING_OUT:
			var bodies = CollisionUtil.bodies_except_player(area_2d.get_overlapping_bodies())
			
			if not bodies.is_empty():
				hit_body.emit(bodies[0])
				enter_state(State.STUCK)
				return
				
			outgoing_progress += delta
			global_position += current_velocity * max_speed * delta

			if outgoing_progress >= outgoing_threshold:
				enter_state(State.GOING_BACK)

		State.GOING_BACK:
			var bodies = area_2d.get_overlapping_bodies()
			
			if CollisionUtil.bodies_contain_player(bodies):
				collided_with_player.emit()
				queue_free()
				return
			
			if not bodies.is_empty():
				hit_body.emit(bodies[0])
				enter_state(State.STUCK)
				return
			
			var direction_to_player: Vector2 = GameManager.player.global_position - global_position
			current_velocity += direction_to_player.normalized() * angular_velocity_multiplier
			if current_velocity.length() > 1.0:
				current_velocity = current_velocity.normalized()

			global_position += current_velocity * max_speed * delta

		State.STUCK:
			pass
	
