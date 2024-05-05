extends Node2D

@onready var animatable_body := $AnimatableBody2D

@export var end_position: Node2D

@export var speed : float = 20


func _ready():
	if not end_position:
		print("Moving platform is missing end position.")
		get_tree().quit()

	var duration = global_position.distance_to(end_position.global_position) / speed
	
	var tween = create_tween()
	tween.set_process_mode(Tween.TWEEN_PROCESS_PHYSICS)
	tween.set_parallel(false)
	tween.set_trans(Tween.TRANS_SINE)
	tween.set_loops()
	tween.tween_property(animatable_body, "global_position", end_position.global_position, duration)
	tween.tween_property(animatable_body, "global_position", global_position, duration)
	
