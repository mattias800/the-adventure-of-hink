extends Node

@onready var camera := %Camera

func connect_to_level(level_node: Node2D, next_tilemap: TileMap) -> void:
	camera.connect_to_level(level_node, next_tilemap)
