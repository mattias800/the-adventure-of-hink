extends Node

@onready var camera := $Camera

func connect_to_tilemap(next_tilemap: TileMap) -> void:
	camera.connect_to_tilemap(next_tilemap)
