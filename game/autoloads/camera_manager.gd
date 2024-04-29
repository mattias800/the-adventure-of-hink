extends Node

var camera: Camera2D

func connect_to_tilemap(next_tilemap: TileMap) -> void:
	camera.connect_to_tilemap(next_tilemap)

func set_camera_node(c: Camera2D):
	camera = c
