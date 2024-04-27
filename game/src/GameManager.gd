extends Node

const LdtkUtil = preload("res://src/utils/LdtkUtil.gd")

@onready var platform_player := %Player
@onready var overworld_player := %OverworldPlayer
@onready var camera := %Camera
@onready var level_manager := %LevelManager

var player

func set_current_player(p: Node2D):
	player = p

func _process(_delta):
	if player:
		level_manager.check_for_level_change(player.global_position)

func teleport_player_to_level(level_name: String, next_global_position_for_player: Vector2):
	print("teleport_player_to_level")
	var level_node = LdtkUtil.find_level_node(get_tree().root, level_name)
	var settings = LdtkUtil.get_level_settings(level_node)
	print(level_node)
	print(settings)
	camera.connect_to_platform_level(level_name)
	if settings["LevelType"] == "Overworld":
		overworld_player.global_position = next_global_position_for_player
		set_current_player(overworld_player)
		camera.set_camera_target(overworld_player)
		overworld_player.enable()
		platform_player.disable()
	if settings["LevelType"] == "Platform":
		platform_player.global_position = next_global_position_for_player
		set_current_player(platform_player)
		camera.set_camera_target(platform_player)
		overworld_player.disable()
		platform_player.enable()
