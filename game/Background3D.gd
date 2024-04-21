extends Sprite2D

func _process(_delta):
	var t = $SubViewport.get_texture()
	texture = t
