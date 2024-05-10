extends Node2D

@onready var animated_sprite_2d = $AnimatedSprite2D
@onready var talkable = $Talkable

const LITTLE_MUSHROOM = preload("res://entities/characters/little_mushroom/little_mushroom.dialogue")

func _ready():
	animated_sprite_2d.play("idle")

func _process(delta):
	if GameManager.player.global_position < global_position:
		animated_sprite_2d.flip_h = true
	else:
		animated_sprite_2d.flip_h = false


func _on_talkable_on_talk():
	if GameState.once_for_character("little_mushroom", "has_met"):
		await CutsceneManager.start_timeline(LITTLE_MUSHROOM, "start")
	else:
		await CutsceneManager.start_timeline(LITTLE_MUSHROOM, "second")
	talkable.activate()
