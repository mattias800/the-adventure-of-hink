extends Node2D

func _ready():
	print("Starting the game.")

	GameManager.player.switch_to_platform()
	CutsceneManager.transition_in()
	GameManager.player.enable()
	# camera.connect_to_platform_level("Level_0")
	#camera.connect_to_overworld_level("Level_5")
	#player.switch_to_overworld()


func _process(_delta):
	if Input.is_action_just_pressed("exit_game"):
		get_tree().quit()

