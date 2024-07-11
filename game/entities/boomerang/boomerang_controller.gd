extends Node2D

@export var boomerang_scene: PackedScene

var spawn_position_offset := 8
var position_offset_from_player_feet := Vector2(0, -4)
var direction_when_player_idle := Vector2(1, 0)

var boomerang_instance : Node2D = null

func _ready():
	pass

func _process(delta):
	if Input.is_action_just_pressed("throw"):
		var held_direction = Input.get_vector("move_left", "move_right", "move_up", "move_down")
		spawn_boomerang(held_direction)

func spawn_boomerang(held_direction: Vector2):
	var direction = held_direction if held_direction.length() > 0.1 else direction_when_player_idle
	if boomerang_instance != null and is_instance_valid(boomerang_instance):
		if boomerang_instance.state == Boomerang.State.STUCK:
			boomerang_instance.queue_free()
		else:
			return
	boomerang_instance = boomerang_scene.instantiate()
	boomerang_instance.global_position = get_spawn_global_position(direction)
	get_tree().root.add_child(boomerang_instance)
	
	boomerang_instance.throw(direction)
		
func _on_player_player_turned(player_direction: String):
	if (player_direction == "left"):
		direction_when_player_idle = Vector2(-1, 0)
	else:
		direction_when_player_idle = Vector2(1, 0)

func get_spawn_global_position(direction: Vector2) -> Vector2:
	var start_position = global_position + (direction * spawn_position_offset)
	return start_position + position_offset_from_player_feet
