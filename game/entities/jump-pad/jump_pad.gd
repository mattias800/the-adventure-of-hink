extends Node2D

@export var power: int = 400

@onready var animated_sprite = $AnimatedSprite2D
@onready var area = $Area2D

func _ready():
	animated_sprite.play("idle")
	
func _on_area_2d_area_shape_entered(area_rid, area, area_shape_index, local_shape_index):
	if area.is_in_group("player_bounce"):
		animated_sprite.stop()
		animated_sprite.play("blam")
		animated_sprite.animation_looped.connect(on_hit_animation_done)
		GameManager.player.trigger_force(Vector2(0, -power))
		#active = false
		#await get_tree().create_timer(0.1).timeout
		#active = true

func on_hit_animation_done():
	animated_sprite.play("idle")
	animated_sprite.animation_finished.disconnect(on_hit_animation_done)
