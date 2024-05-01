extends Node2D

func _ready():
	print("Starting the game.")

	GameManager.player.switch_to_platform()
	CutsceneManager.transition_in()
	GameManager.player.enable()

func _process(_delta):
	if Input.is_action_just_pressed("exit_game"):
		get_tree().quit()

