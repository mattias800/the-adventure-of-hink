extends Node2D
class_name WaterSpring

var velocity := 0.0
var force := 0.0
var height := 0.0
var target_height := 0.0

func initialize(next_global_position: Vector2i):
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
