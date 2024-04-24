extends Node2D

@onready var platform_player := %Player
@onready var overworld_player := %OverworldPlayer
@onready var level_manager := %LevelManager
@onready var camera := %Camera

func _ready():
	print("Starting the game.")
	# camera.connect_to_platform_level("Level_1")
	# level_manager.set_current_player(platform_player)
	# camera.set_camera_target(platform_player)
	camera.connect_to_overworld_level("Level_5")
	level_manager.set_current_player(overworld_player)
	camera.set_camera_target(overworld_player)

func _process(_delta):
	if Input.is_action_just_pressed("exit_game"):
		get_tree().quit()
