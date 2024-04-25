extends ColorRect

@onready var camera = %Camera

var anchor: Vector2

func _process(_delta):
	var scale = 0.8
	var pos = camera.global_position - anchor
	pos.x /= 320 * scale
	pos.y /= 180 * scale
	material.set("shader_parameter/offset", pos)
	pass

func reset_position():
	anchor = camera.global_position
