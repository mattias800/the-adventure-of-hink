extends Node

const LdtkUtil = preload("res://src/utils/LdtkUtil.gd")

var player
var camera

var active_level_type
var active_respawn_global_position

func _ready():
	LevelManager.player_entered_level.connect(_on_level_manager_player_entered_level)
	LevelManager.player_exited_level.connect(_on_level_manager_player_exited_level)
	LevelManager.player_entered_portal.connect(_on_level_manager_player_entered_portal)
	LevelManager.player_entered_out_of_bounds.connect(_on_level_manager_player_entered_out_of_bounds)

func _process(_delta):
	if player:
		LevelManager.check_for_level_change(player.global_position)
	else:
		print("GameManager has no player.")

func set_player_node(p: Node2D):
	player = p
	CutsceneManager.set_player_node(player)

func set_camera_node(p: Node2D):
	camera = p
	CameraManager.set_camera_node(camera)
	CutsceneManager.set_camera_node(camera)

func _on_level_manager_player_entered_level(tilemap: TileMap):
	print("_on_level_manager_player_entered_tilemap")
	var level_type = LevelManager.get_level_type_for_tilemap(tilemap)

	# Store player position for respawn after death
	active_respawn_global_position = player.global_position

	var use_transition = active_level_type and active_level_type != level_type

	CameraManager.connect_to_tilemap(tilemap)

	match level_type:
		LevelManager.LevelType.OVERWORLD:
			print("Entered overworld map")
			# environment_manager.set_background_enabled(false)
			player.switch_to_overworld()
			active_level_type = level_type
		LevelManager.LevelType.PLATFORM:
			print("Entered platform map")
			# environment_manager.set_background_enabled(true)
			player.switch_to_platform()
			active_level_type = level_type

	if use_transition:
		CutsceneManager.transition_in()
		player.enable()

func _on_level_manager_player_exited_level(tilemap: TileMap):
	pass # Replace with function body.


func _on_cutscene_manager_cutscene_started():
	player.disable()


func _on_cutscene_manager_cutscene_ended():
	player.enable()



func _on_level_manager_player_entered_portal(target_level, next_global_position_for_player):
	player.disable()
	await CutsceneManager.transition_out()
	await get_tree().create_timer(0.5).timeout
	player.global_position = next_global_position_for_player
	camera.jump_to_target()

func _on_level_manager_player_entered_out_of_bounds():
	if active_respawn_global_position:
		player.global_position = active_respawn_global_position
