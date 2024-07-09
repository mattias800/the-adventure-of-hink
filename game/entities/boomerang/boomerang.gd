extends CharacterBody2D

@onready var sprite_2d = $Sprite2D

var start_direction := Vector2(1, 0)
var current_direction := Vector2(1, 0)
var max_distance := 5.0
var max_speed := 200.0
var current_speed := 1.0
var rotation_speed := 4.0
var rotating := true

var start_position := Vector2(0, 0)

# Called when the node enters the scene tree for the first time.
func _ready():
	throw(global_position, start_direction)

func throw(start: Vector2, direction: Vector2):
	start_position = start
	global_position = start_position
	start_direction = direction
	current_direction = direction
	current_speed = max_speed
	# velocity = direction * speed
	rotating = true

func _process(delta):
	if rotating:
		sprite_2d.rotate(PI * rotation_speed * delta)

func _physics_process(delta):
	var collision = move_and_collide(current_direction * current_speed * delta)
	if collision != null:
		rotating = false
	
