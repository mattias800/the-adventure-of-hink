extends Node2D

@onready var animated_sprite_2d: AnimatedSprite2D = $AnimatedSprite2D

func _ready() -> void:
	pass
	
func _on_interactible_on_interact() -> void:
	animated_sprite_2d.play("opening")
	await animated_sprite_2d.animation_looped
	animated_sprite_2d.play("open")
