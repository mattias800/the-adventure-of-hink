extends Node

const LdtkUtil = preload("res://src/utils/LdtkUtil.gd")

@onready var player := %Player
@onready var camera := %Camera
@onready var level_manager := %LevelManager
@onready var camera_manager := %CameraManager
@onready var environment_manager = %EnvironmentManager

func _process(_delta):
	level_manager.check_for_level_change(player.global_position)

func teleport_player_to_level(level_name: String, next_global_position_for_player: Vector2):
	print("teleport_player_to_level")
	var level_node = LdtkUtil.find_level_node(get_tree().root, level_name)
	var settings = LdtkUtil.get_level_settings(level_node)
	print(level_node)
	print(settings)
	camera.connect_to_platform_level(level_name)
	player.global_position = next_global_position_for_player


func _on_level_manager_player_entered_tilemap(level_name: String, tilemap: TileMap, metadata: Dictionary):
	print("_on_level_manager_player_entered_tilemap")
	environment_manager.fog(metadata["FogEnabled"])
	environment_manager.overworld_clouds(metadata["OverheadCloudsEnabled"])
	camera_manager.connect_to_level(level_name, tilemap)

	match metadata["LevelType"]:
		"Overworld":
			player.switch_to_overworld()
		"Platform":
			player.switch_to_platform()
