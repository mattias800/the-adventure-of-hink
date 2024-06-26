extends Area2D
class_name Room

signal on_player_entered_room
signal on_player_exited_room

@export var camera: Camera2D
@export var collision_shape: CollisionShape2D

func _physics_process(delta):
	if overlaps_body(GameManager.player):
		CameraLimiter.apply_collision_shape_to_camera_limits(camera, collision_shape)
	
func _on_body_entered(body):
	if camera == null:
		print("Missing camera on room node.")

	if collision_shape == null:
		print("Missing collision shape on room node.")

	if body.is_in_group("player"):
		print("Player entered room: " + name)
		CameraLimiter.apply_collision_shape_to_camera_limits(camera, collision_shape)
		on_player_entered_room.emit()

func _on_body_exited(body):
	if body.is_in_group("player"):
		on_player_exited_room.emit()
