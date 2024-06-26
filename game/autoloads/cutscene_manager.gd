extends Node

signal cutscene_started
signal cutscene_ended

@onready var transition_rect = $"CanvasLayer/TransitionRect"
@onready var animation_player = $"CanvasLayer/TransitionRect/AnimationPlayer"

var cutscene_playing: bool = false

func _ready():
	pass
	
func _physics_process(_delta):
	var focus = GameManager.player.global_position - CameraManager.camera.global_position
	transition_rect.material.set_shader_parameter("focus_pos", focus)

func start_timeline(resource, start):
	cutscene_playing = true
	cutscene_started.emit()
	GameManager.player.disable()
	DialogueManager.show_dialogue_balloon(resource, start)
	await DialogueManager.dialogue_ended
	await get_tree().create_timer(0.25).timeout # Prevent last input to be sent to player.
	cutscene_ended.emit()
	GameManager.player.enable()
	cutscene_playing = false

func transition_in():
	print("transition_in")
	animation_player.play("Transition")

func transition_out():
	print("transition_out")
	animation_player.play_backwards("Transition")
	await animation_player.animation_finished
