extends Node

const LdtkUtil = preload("res://src/utils/LdtkUtil.gd")

@onready var player := %Player
@onready var camera := %Camera
@onready var level_manager := %LevelManager
@onready var camera_manager := %CameraManager
@onready var environment_manager = %EnvironmentManager

var active_level_type

func _process(_delta):
	level_manager.check_for_level_change(player.global_position)

func teleport_player_to_level(level_name: String, next_global_position_for_player: Vector2):
	print("teleport_player_to_level")
	var level_node = LdtkUtil.find_level_node(get_tree().root, level_name)
	var settings = LdtkUtil.get_level_settings(level_node)
	print(level_node)
	print(settings)
	player.global_position = next_global_position_for_player


func _on_level_manager_player_entered_level(level_node, tilemap, metadata):
	print("_on_level_manager_player_entered_tilemap")
	var level_type = metadata["LevelType"]

	if (active_level_type and active_level_type != level_type):
		print("Switch level type, transition!")
		# TODO Transition
		
	environment_manager.set_fog_visible(metadata["FogEnabled"])
	environment_manager.set_overworld_clouds_visible(metadata["OverheadCloudsEnabled"])
	camera_manager.connect_to_level(level_node, tilemap)
	
	
	match level_type:
		"Overworld":
			environment_manager.set_background_enabled(false)
			player.switch_to_overworld()
			active_level_type = level_type
		"Platform":
			environment_manager.set_background_enabled(true)
			player.switch_to_platform()
			active_level_type = level_type


func _on_level_manager_player_exited_level():
	pass # Replace with function body.


func _on_cutscene_manager_cutscene_started():
	player.disable()


func _on_cutscene_manager_cutscene_ended():
	player.enable()

