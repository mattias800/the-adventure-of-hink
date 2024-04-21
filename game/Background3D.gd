extends Sprite2D

func _process(delta):
	var t = $SubViewport.get_texture()
	texture = t
