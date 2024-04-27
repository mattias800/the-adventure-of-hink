extends Node2D

@onready var player := %Player
@onready var level_manager := %LevelManager
@onready var game_manager := %GameManager
@onready var cutscene_manager := %CutsceneManager
@onready var camera := %Camera

func _ready():
	print("Starting the game.")
	
	camera.set_camera_target(player)
	player.switch_to_platform()

	cutscene_manager.transition_in()
	player.enable()
	# camera.connect_to_platform_level("Level_0")
	#camera.connect_to_overworld_level("Level_5")
	#player.switch_to_overworld()


func _process(_delta):
	if Input.is_action_just_pressed("exit_game"):
		get_tree().quit()

