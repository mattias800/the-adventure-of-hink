extends Node

const LdtkUtil = preload("res://src/utils/LdtkUtil.gd")

@onready var player = $Player

var active_level_type
var active_respawn_global_position

var is_entering_new_scene: bool = true
var new_scene_portal_name = "YayPortal"
var scene_is_loaded = false

func _ready():
	# LevelManager.player_entered_level.connect(_on_level_manager_player_entered_level)
	LevelManager.player_exited_level.connect(_on_level_manager_player_exited_level)
	LevelManager.player_entered_out_of_bounds.connect(_on_level_manager_player_entered_out_of_bounds)
	CameraManager.camera.set_camera_target(player)
	CutsceneManager.cutscene_started.connect(_on_cutscene_manager_cutscene_started)
	CutsceneManager.cutscene_ended.connect(_on_cutscene_manager_cutscene_ended)

func _process(_delta):
	if not scene_is_loaded:
		is_entering_new_scene = true
		scene_is_loaded = true
		get_tree().change_scene_to_file("res://world/world_01/level_01/level_01.tscn")
		return

	if Input.is_action_just_pressed("exit_game"):
		get_tree().quit()
		return

	if is_entering_new_scene:
		print("enter new scene!")
		is_entering_new_scene = false
		enter_new_scene()
		return

	LevelManager.check_for_level_change(player.global_position)

func on_player_entered_portal(portal):
	print("on_player_entered_portal")
	player.disable()
	await CutsceneManager.transition_out()
	await get_tree().create_timer(0.5).timeout
	is_entering_new_scene = true
	new_scene_portal_name = portal.target_portal_name
	print("Load next level?")
	if portal.next_scene_path:
		print("LOAD SCENE WOO")
		LevelManager.active_room_tilemap = null
		load_scene(portal.next_scene_path)


func enter_new_scene():
	var portal = get_portal_by_name(new_scene_portal_name)

	if not portal:
		print("Found no target portal: " + new_scene_portal_name)
		print("Available portals:")
		print(get_tree().get_nodes_in_group("portals"))
		print("Scene:")
		print(get_tree().root.get_children())
		get_tree().quit()
		return

	print("Found portal: " + portal.name)
	var spawn_point = portal.spawn_point
	if spawn_point:
		print("Found spawn point")
		print(spawn_point.global_position)
		player.global_position = spawn_point.global_position
	else:
		print("Found no spawn point")
		player.global_position = portal.global_position

	print(player.global_position)
	CameraManager.camera.jump_to_target()
	CutsceneManager.transition_in()
	
	if get_tree().current_scene.is_in_group("platformers"):
		player.switch_to_platform()
	elif get_tree().current_scene.is_in_group("overworlds"):
		player.switch_to_overworld()
	else:
		print("Scene must be in group platformers or overworlds.")
		get_tree().quit()
		
	player.enable()

func get_portal_by_name(name_: String):
	var portals = get_tree().get_nodes_in_group("portals")
	for p in portals:
		if p.name == name_:
			return p

func on_player_entered_room(tilemap: TileMap):
	print("on_player_entered_room")

	# Store player position for respawn after death
	active_respawn_global_position = player.global_position

	CameraManager.connect_to_tilemap(tilemap)


func _on_level_manager_player_exited_level(tilemap: TileMap):
	pass # Replace with function body.


func _on_cutscene_manager_cutscene_started():
	player.disable()

func _on_cutscene_manager_cutscene_ended():
	player.enable()

func _on_level_manager_player_entered_out_of_bounds():
	print("Player is out of bounds.")
	if active_respawn_global_position:
		player.global_position = active_respawn_global_position

func load_scene(path):
	get_tree().current_scene.queue_free() # Instead of free()
	var packed_scene := ResourceLoader.load(path) as PackedScene
	var instanced_scene := packed_scene.instantiate()
	# Add it to the scene tree, as direct child of root
	get_tree().root.add_child(instanced_scene)
	# Set it as the current scene, only after it has been added to the tree
	get_tree().current_scene = instanced_scene
