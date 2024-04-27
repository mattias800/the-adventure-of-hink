extends Node

const LdtkUtil = preload("res://src/utils/LdtkUtil.gd")

@onready var player := %Player
@onready var camera := %Camera
@onready var level_manager := %LevelManager
@onready var camera_manager := %CameraManager
@onready var cutscene_manager := %CutsceneManager
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
	
	# Do transition here, before moving player position.
	player.disable()
	await cutscene_manager.transition_out()
	await get_tree().create_timer(0.5).timeout
	
	player.global_position = next_global_position_for_player
	camera.jump_to_target()


func _on_level_manager_player_entered_level(level_node, tilemap, metadata):
	print("_on_level_manager_player_entered_tilemap")
	var level_type = metadata["LevelType"]

	var use_transition = active_level_type and active_level_type != level_type
	
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

	if use_transition:
		cutscene_manager.transition_in()
		player.enable()

func _on_level_manager_player_exited_level(level_node, tilemap, metadata):
	pass # Replace with function body.


func _on_cutscene_manager_cutscene_started():
	player.disable()


func _on_cutscene_manager_cutscene_ended():
	player.enable()



func _on_level_manager_player_entered_portal(target_level, next_position_for_player):
	teleport_player_to_level(target_level.name, next_position_for_player)
