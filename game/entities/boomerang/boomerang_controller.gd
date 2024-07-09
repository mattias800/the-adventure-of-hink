extends Node2D

@export var boomerang_scene: PackedScene

var spawn_position_offset := Vector2(8, 0)
var right_offset := Vector2(8, -4)
var left_offset := Vector2(-8, -4)
var direction := Vector2(1, 0)

func _ready():
	pass

func _process(delta):
	if Input.is_action_just_pressed("throw"):
		spawn_boomerang()

func spawn_boomerang():
	var boomerang_instance = boomerang_scene.instantiate()
	boomerang_instance.global_position = global_position + spawn_position_offset
	get_tree().root.add_child(boomerang_instance)
	boomerang_instance.throw(direction)

func _on_player_player_turned(player_direction: String):
	if (player_direction == "left"):
		spawn_position_offset = left_offset
		direction = Vector2(-1, 0)
	else:
		spawn_position_offset = right_offset
		direction = Vector2(1, 0)
