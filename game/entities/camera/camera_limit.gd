extends Node2D

@export var camera: Camera2D
@export var collision_shape: CollisionShape2D

func _on_body_entered(body: Node2D) -> void:
	if body.is_in_group("player"):
		print("Player entered camera limit: ", name)
		print(collision_shape.shape.get_rect())
		var rect = collision_shape.shape.get_rect()
		rect.position = collision_shape.global_position + rect.position # + Vector2(320, 180) / 2
		print(rect)
		camera.set_limit(SIDE_LEFT, rect.position.x)
		camera.set_limit(SIDE_RIGHT, rect.position.x + rect.size.x)
		camera.set_limit(SIDE_TOP, rect.position.y)
		camera.set_limit(SIDE_BOTTOM, rect.position.y + rect.size.y)
		
