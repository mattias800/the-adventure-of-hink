extends ColorRect

var anchor: Vector2

func _process(_delta):
	var scroll_scale = 0.8
	var pos = CameraManager.Camera.global_position - anchor
	pos.x /= 320 * scroll_scale
	pos.y /= 180 * scroll_scale
	material.set("shader_parameter/offset", pos)
	pass

func reset_position():
	anchor = CameraManager.camera.global_position
