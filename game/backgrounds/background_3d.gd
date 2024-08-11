extends Node2D

func _process(_delta):
	%Sprite2D.texture = %SubViewport.get_texture()
