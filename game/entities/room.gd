extends Area2D
class_name Room

signal on_player_entered_room
signal on_player_exited_room

@onready var collision_shape = $CollisionShape2D

@export var enabled := true

func _physics_process(delta):
	if not enabled:
		return
	
	if collision_shape == null:
		print("Missing collision shape on room node.")
		return

	if enabled and overlaps_body(GameManager.player):
		CameraLimiter.apply_collision_shape_to_camera_limits(CameraManager.camera, collision_shape)
	
func _on_body_entered(body):
	if enabled and body.is_in_group("player"):
		print("Player entered room: " + name)
		on_player_entered_room.emit()

func _on_body_exited(body):
	if enabled and body.is_in_group("player"):
		on_player_exited_room.emit()
