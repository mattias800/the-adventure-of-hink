extends Node

var camera

func connect_to_tilemap(next_tilemap: TileMap) -> void:
	camera.connect_to_tilemap(next_tilemap)

func set_camera_target(target: Node2D):
	camera.set_camera_target(target)

func set_camera(c):
	camera = c
	
