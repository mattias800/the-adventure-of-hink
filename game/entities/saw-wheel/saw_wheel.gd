extends Node2D

@export var rotation_speed := 3.0

func _process(delta: float) -> void:
	rotate(PI * rotation_speed * delta)


func _on_area_2d_body_entered(body: Node2D) -> void:
	if CollisionUtil.is_player(body):
		GameManager.respawn_player()
