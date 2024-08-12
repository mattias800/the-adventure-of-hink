extends Node2D
class_name WaterSpring

@export var splash_particles: PackedScene
@export var splash_sound: PackedScene
@onready var collision_shape_2d = $Area2D/CollisionShape2D

var velocity := 0.0
var force := 0.0
var height := 0.0
var target_height := 0.0
var index := 0

var motion_factor = 0.0025

signal splash(index: int, speed: float)

func initialize(next_global_position: Vector2i, idx: int):
	index = idx
	global_position = next_global_position
	height = position.y
	target_height = position.y
	velocity = 0.0
	
func water_update(spring_constant: float, dampening: float):
	height = position.y
	
	var x := height - target_height

	var loss := -dampening * velocity
	
	# Hooke's law
	force = - spring_constant * x + loss
	
	velocity += force
	
	position.y += velocity

func set_collision_width(value: float):
	var size = collision_shape_2d.shape.size
	var new_size = Vector2(value, 4)
	collision_shape_2d.shape.set_size(new_size)
	
	


func _on_area_2d_body_entered(body):
	if "velocity" in body:
		var speed = body.velocity.y * motion_factor
		var s = splash_particles.instantiate()
		var sound = splash_sound.instantiate()
		add_child(s)
		add_child(sound)
		s.emitting = true
		splash.emit(index, speed)
	
