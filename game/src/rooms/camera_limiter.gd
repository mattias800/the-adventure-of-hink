class_name CameraLimiter

static func apply_collision_shape_to_camera_limits(camera: Camera2D, collision_shape: CollisionShape2D) -> void:
		var rect = collision_shape.shape.get_rect()
		rect.position = collision_shape.global_position + rect.position
		camera.set_limit(SIDE_LEFT, rect.position.x)
		camera.set_limit(SIDE_RIGHT, rect.position.x + rect.size.x)
		camera.set_limit(SIDE_TOP, rect.position.y)
		camera.set_limit(SIDE_BOTTOM, rect.position.y + rect.size.y)
		
